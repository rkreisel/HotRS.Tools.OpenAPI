# HotRS.Tools.OpenAPI
Various tools and extensions for OpenAPI (3.x) and Scalar

Follow/Contribute:
https://github.com/rkreisel/HotRS.Tools.OpenAPI

## Features

These attributes and transformers enable:

- Addition of a Bearer authentication scheme to the top of the Scalar interface
- Setting a variable in the controller to the value of an enum
- Addition of an "explorer" button to controller methods which upload a file.

## Usage

### Token Input in Scalar UI

In the AddOpenApi block add BearerSecurityRequirementTransformer and BearerSecuritySchemeTransformer. Note that one is an operation transformer and the other is document transformer:

```C#
 builder.Services.AddOpenApi("v1", options =>
 {
     options.AddOperationTransformer<BearerSecurityRequirementTransformer>();
     options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
 });
```

### Defaulting a controller input parameter to an Enum value

**Step 1:** 

Add the ScalarDefaultValueOperationTransformer in the AddOpenApi block

```C#
    services.AddOpenApi("v1", options =>
    {
        options.AddOperationTransformer<ScalarDefaultValueOperationTransformer>();
    });
```

**Step 2:** Add the ScalarDefaultValue attribute to the controller method signature.  
***Note that the attribute ONLY effects the Scalar UI.*** If you want the method to actually default to the enum value, you must set it explicitly in the signature.

In the sample below [ScalarDefaultValue(ImageType.JPG)] only sets the default value in Scalar, the "= ImageType.JPG" value sets the runtime default. (For clarity, you SHOULD always use the same enum value.)

```c#
/// <returns></returns>F
[HttpGet("{accountId:int}")]
[ProducesResponseType(StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status404NotFound)]
public async Task<ActionResult> Get(
    int accountId, 
    [ScalarDefaultValue(ImageType.JPG)] ImageType imgType = ImageType.JPG)
{
    var imgBytes = await _ProfilePicService.GetProfilePicBytes(accountId, imgType);
    if (imgBytes != null)
    {
        return File(imgBytes, "application/octet-stream", $"PP{accountId}.{imgType}");
    }
    return NotFound($"No profile picture was found for userid {accountId}");
}
```

### Adding an Explorer/Finder button to the Scalar UI

There are two transformers that provide the button. One for files under 4mb and one for "large" files.

FormFileOperationTransformer is for small files. It looks for an IFormFile in the method signature.

LargeFileUploadOperationTransformer is for large files.

Implement either or both as needed by adding them to the AddOpenAPI block.

```c#

    services.AddOpenApi("v1", options =>
    {
        // All these transformers are found in Hotrs.Tools.OpenAPI nuget package
        options.AddOperationTransformer<FormFileOperationTransformer>();
        options.AddOperationTransformer<LargeFileUploadOperationTransformer>();
    });
```

Note: LargeFileUploadOperationTransformer also requires the [LargeFileUpload] attribute on the controller method.

```c#
 [HttpPost]
 [DisableFormValueModelBinding]
 [RequestSizeLimit(Constants.MAXUPLOADSIZE)]
 [RequestFormLimits(MultipartBodyLengthLimit = Constants.MAXUPLOADSIZE)]
 [LargeFileUpload]
 public async Task<string> Upload(
     [FromQuery] string filename,
     [FromQuery] bool forceOverwrite = true)
 {
    ... code ...
 }
```

If you for some unknown reason want the UI to display a different name for the file parameter on the UI you can add it as a parameter to the attribute:

```
    [HttpPost]
    [DisableFormValueModelBinding]
    [RequestSizeLimit(Constants.MAXUPLOADSIZE)]
    [RequestFormLimits(MultipartBodyLengthLimit = Constants.MAXUPLOADSIZE)]
    [LargeFileUpload("database")]
    public async Task<string> Upload(
        [FromQuery] string filename,
        [FromQuery] bool forceOverwrite = true)
    {
    	... code ...
    }

```

