# Arazzo 1.0.1 spec review: differences and misses

This report compares the current implementation in `src/lib` and its tests with the Arazzo 1.0.1 specification at <https://spec.openapis.org/arazzo/v1.0.1.html>.

## Executive summary

The library covers the core object model and basic parsing/serialization, but a noticeable part of the Arazzo 1.0.1 conformance surface is either unvalidated or modeled more loosely than the spec requires. The biggest gaps are:

| Priority | Area | Gap |
| --- | --- | --- |
| High | Required fields | Parser/serializer enforcement is incomplete for several required fields and minimum-cardinality rules. |
| High | Step/action invariants | Mutually-exclusive and type-dependent fields are not enforced. |
| High | Parameter rules | Parameter uniqueness is checked incorrectly, and `in` is over-required in one spec-allowed scenario. |
| Medium | Runtime expressions | Validation is only applied to a subset of fields that the spec defines as expressions. |
| Medium | Reusable Object behavior | Extra properties are diagnosed instead of being ignored as required by the spec. |
| Medium | Test coverage | Spec-sample coverage intentionally disables validation, so many conformance gaps would not fail tests. |

## What is implemented well

The implementation does have good coverage for several building blocks:

- top-level document serialization requires `info`, `sourceDescriptions`, and `workflows` to be present and non-empty (`src/lib/Models/ArazzoDocument.cs:72-101`)
- component map keys and output keys are validated against the spec regex (`src/lib/Validation/ArazzoKeyValidator.cs:10-46`)
- step output values are validated as runtime expressions (`src/lib/Models/ArazzoStep.cs:81-125`, `src/lib/Reader/V1/ArazzoStepDeserializer.cs:21-28`)
- reusable non-input references correctly use `reference`, while input references use `$ref` (`src/lib/Models/BaseArazzoReference.cs:129-133`)

## Detailed findings

### 1. Required-field enforcement is incomplete

The parser's post-parse required-field validation is very small. `ParsingContext.ValidateRequiredFields` only checks `info` and then runs workflow-parameter duplicate detection; it does **not** enforce required `sourceDescriptions`, required `workflows`, or the "at least one entry" rule for either array (`src/lib/Reader/ParsingContext.cs:255-301`).

That permissiveness is also baked into tests:

- `ParsingContextTests.Parse_ValidArazzoDocument_ReturnsDocument` accepts a document with `sourceDescriptions: []` and no `workflows` (`tests/lib/Reader/ParsingContextTests.cs:123-139`)
- `ArazzoDocument.LoadFromStreamAsync` / `ParseAsync` tests also treat empty arrays as valid (`tests/lib/Serialization/ArazzoDocumentTests.cs:376-419`)

Additional required-field misses:

- `ArazzoInfo.SerializeAsV1` writes `title` and `version` if present but does not require them, even though both are required by the spec (`src/lib/Models/ArazzoInfo.cs:9-47`)
- `ArazzoWorkflow.SerializeAsV1` only requires `workflowId`; it does not require `steps` (`src/lib/Models/ArazzoWorkflow.cs:72-108`)
- `ArazzoWorkflowTests.SerializeAsV1_MinimalWorkflow_ShouldWriteCorrectJson` explicitly treats a workflow with no `steps` as valid (`tests/lib/Serialization/ArazzoWorkflowTests.cs:119-141`)

### 2. Step target invariants are not enforced

The spec makes `operationId`, `operationPath`, and `workflowId` mutually exclusive, with a step expected to target an operation or another workflow.

The implementation currently just serializes whichever of the three properties happen to be set (`src/lib/Models/ArazzoStep.cs:77-107`) and deserializes all three independently (`src/lib/Reader/V1/ArazzoStepDeserializer.cs:9-29`).

This is not just theoretical: `SerializationErrorPathTests` asserts that a step can serialize with **all three** fields at once (`tests/lib/Serialization/SerializationErrorPathTests.cs:100-120`).

Related misses:

- no validation that at least one of those three targeting fields is present
- no validation that `requestBody` is only used with operation-based steps
- no validation that referenced `workflowId` / `operationId` / `operationPath` actually resolve as required by the spec

### 3. Success/failure action invariants are too loose

`ArazzoResultAction.SerializeCommonPropertiesAsV1` always writes `workflowId` and `stepId` when present; there is no mutual-exclusion or type-based validation (`src/lib/Models/ArazzoResultAction.cs:33-57`).

The tests confirm this looser shape:

- success `goto` with both `workflowId` and `stepId` is considered valid (`tests/lib/Serialization/ArazzoSuccessActionTests.cs:13-61`)
- failure `goto` with both `workflowId` and `stepId` is considered valid (`tests/lib/Serialization/ArazzoFailureActionTests.cs:93-121`)

Failure-action-specific misses:

- `retryAfter` / `retryLimit` are serialized whenever set, regardless of action type (`src/lib/Models/ArazzoFailureAction.cs:22-40`)
- there is no enforcement that `retryAfter` and `retryLimit` are non-negative
- there is no implementation of the spec default `retryLimit = 1` when omitted
- deserialization silently ignores invalid numeric text instead of reporting a diagnostic (`src/lib/Reader/V1/ArazzoFailureActionDeserializer.cs:20-34`)

### 4. Parameter rules are partly modeled incorrectly

The spec defines parameter uniqueness by the **combination** of `name` and `in`, but the only duplicate check in the repository uses `name` alone (`src/lib/Reader/ParsingContext.cs:286-297`).

There is a test that explicitly expects same-name parameters in different locations to be treated as duplicates:

- header/query pair both named `token` is flagged as duplicate (`tests/lib/Reader/ParsingContextTests.cs:177-202`)

Other parameter-related misses:

- duplicate detection only runs for **workflow-level** parameters; there is no analogous step-level validation (`src/lib/Reader/ParsingContext.cs:268-301`)
- `ArazzoParameter.SerializeAsV1` always requires `in` (`src/lib/Models/ArazzoParameter.cs:29-45`), but the spec allows it to be omitted when the enclosing step targets a `workflowId`
- the parser/deserializer accepts missing `in` but does not validate the conditional rule around when it must be present (`src/lib/Reader/V1/ArazzoParameterDeserializer.cs:9-21`)

### 5. Workflow output values are not validated as runtime expressions

Step outputs are validated for both key format and runtime-expression values:

- serializer: `src/lib/Models/ArazzoStep.cs:81-125`
- deserializer: `src/lib/Reader/V1/ArazzoStepDeserializer.cs:21-28`
- tests cover invalid output values (`tests/lib/Serialization/ArazzoStepTests.cs:243-280`)

Workflow outputs only get key validation:

- serializer: `src/lib/Models/ArazzoWorkflow.cs:98-104`
- deserializer: `src/lib/Reader/V1/ArazzoWorkflowDeserializer.cs:30-37`
- tests only cover invalid keys, not invalid values (`tests/lib/Serialization/ArazzoWorkflowTests.cs:233-269`)

That leaves a direct spec miss: workflow output values are also expressions, but the library does not validate them.

### 6. Runtime-expression validation is only wired into a narrow subset of fields

`ArazzoRuntimeExpressionValidator` exists, but it is only used for step outputs (`src/lib/Validation/ArazzoRuntimeExpressionValidator.cs:11-66`, `src/lib/Models/ArazzoStep.cs:82-83`, `src/lib/Reader/V1/ArazzoStepDeserializer.cs:27`).

I did not find equivalent validation for other expression-bearing fields called out by the spec, including:

- `Reusable Object.reference`
- parameter `value`
- request-body `payload`
- payload-replacement `value`
- criterion `context`
- `dependsOn` values when they reference external workflows via runtime expressions
- step `operationPath` when it embeds a source-description runtime expression

This does not necessarily break parsing/serialization, but it means a large share of the spec's expression surface is currently unchecked.

### 7. Criterion conditional rules are under-validated

`ArazzoCriterion` deserialization only maps `context`, `condition`, and `type`; there is no rule enforcement beyond recognizing the shape of `type` (`src/lib/Reader/V1/ArazzoCriterionDeserializer.cs:7-27`).

A concrete miss from the spec:

- regex/jsonpath/xpath criteria require `context`, but `ArazzoCriterionTests.Deserialize_StringTypeAsRegex_ShouldCreateCriterionWithRegexType` accepts `type: "regex"` with no `context` and no diagnostic (`tests/lib/Serialization/ArazzoCriterionTests.cs:241-259`)

### 8. Request Body is stricter than the spec

Per the spec, `contentType` and `payload` are optional fields on the Request Body Object.

The implementation requires both during serialization:

- `ArgumentException.ThrowIfNullOrEmpty(ContentType);`
- `ArgumentNullException.ThrowIfNull(Payload);`

See `src/lib/Models/ArazzoRequestBody.cs:35-45`.

That means valid spec shapes like:

- request bodies with only `payload`
- request bodies with only `replacements`
- request bodies that rely on the target operation's content type

cannot be serialized by the current model.

### 9. Reusable Object extra properties are treated as errors instead of being ignored

The spec says Reusable Objects cannot be extended and that extra properties **must be ignored**.

In practice, unknown properties fall through to the generic invalid-property diagnostic path in `JsonNodeHelper.ParseField` (`src/lib/Reader/JsonNodeHelper.cs:179-205`), because `ReusableObjectPatternFields` is empty (`src/lib/Reader/V1/ArazzoReusableObjectDeserializer.cs:7-22`).

The current test suite codifies that behavior:

- `ArazzoReusableObjectTests.Deserialize_ShouldRejectExtensions` expects `"x-flag is not a valid property"` (`tests/lib/Serialization/ArazzoReusableObjectTests.cs:120-136`)

That is stricter than the spec text, which calls for ignore-on-read behavior rather than a diagnostic.

### 10. Conformance-oriented tests currently leave most validation disabled

The main spec-sample parsing test uses `ValidationRuleSet.GetEmptyRuleSet()` before loading all copied sample Arazzo files (`tests/lib/Reader/ArazzoSpecificationExamplesTests.cs:13-18`).

Combined with the very small custom validation surface in `ParsingContext.ValidateRequiredFields` (`src/lib/Reader/ParsingContext.cs:255-301`) and generic `document.Validate(...)` usage in `ArazzoJsonReader` (`src/lib/Reader/ArazzoJsonReader.cs:55-72`), this means many structural conformance gaps would not fail the current sample-based coverage.

## Suggested remediation order

If this repository wants closer 1.0.1 conformance, I would address the gaps in this order:

1. Add structural validation for required fields and minimum-cardinality rules (`info.title`, `info.version`, document arrays, workflow `steps`).
2. Enforce step/action mutual exclusion and type-dependent rules.
3. Fix parameter validation to use `(name, in)` uniqueness and allow omitted `in` for workflow-targeted steps.
4. Extend runtime-expression validation to workflow outputs and the other spec-defined expression fields.
5. Relax Reusable Object extra-property handling from diagnostic to ignore-on-read.
6. Add spec-focused tests with validation enabled, especially for 1.0.1-only expectations and failure cases.

## Bottom line

The implementation is a solid parser/serializer foundation, but today it is closer to a permissive Arazzo object model than a strict Arazzo 1.0.1 conformance layer. The main misses are not around basic object presence, but around **spec invariants, conditional requirements, and validation coverage**.
