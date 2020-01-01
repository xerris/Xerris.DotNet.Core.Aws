# Xerris.DotNet.Core.Aws
AWS shared components

This module contains reusable components for C# AWS Lambdas and other AWS resources

### ApiGatewayProxyRequestExtensions
The ApiGatewayProxyRequestExtensions class provides convenience methods for worjking with the APIGatewayProxRequest 

* Parse<T>          - Deserializes the JSON body into a C# Dto

```C#
    var addUserRequest = apiRequest.Parse<AddUserRequest>();
```

* GetQueryString    - Retrieves the value of a querystring based on the key provided 
                      from the APIGatewayProxyRequest queryString dictionary
                      
```C#
    var searchString = apiRequest.GetQueryString("search_string");
```                      
                      
* GetAuthorization  - Retrieves the Authorization header from the APIGatewayProxyRequest

```C#
    var searchString = apiRequest.GetAuthorization();
``` 

### ResponseBuilder
Factory method extensions that easily create the APIGatewayResponse 

* OK<T>  - Creates a HTTP 200 (OK) response with a given payload in JSON format
```C#
    var results = await service.GetListByIdAndDescriptionAsync(request.GetQueryString("search_string"));
    return results.Ok();
``` 

* Created<T>  - Creates a HTTP 201 (CREATED) response with a given payload in JSON format
```C#
    var response = await userManagementService.AddUserAsync(addUserRequest).ConfigureAwait(false);
    return response.Created();
``` 

* Accepted<T>  - Creates a HTTP 202 (ACCEPTED) response with a given payload in JSON format
```C#
    var response = await userManagementService.DeleteUserAsync(addUserRequest).ConfigureAwait(false);
    return response.Accepted();
``` 

* Error(this string error) - Creates a HTTP 500 (SERVER_ERROR) response with a given payload in JSON format
```C#
    var response = await userManagementService.DeleteUserAsync(addUserRequest).ConfigureAwait(false);
    return response.Accepted();
``` 

* UnAuthorized<T> - Creates a HTTP 400 (BAD_REQUEST) response with a given payload in JSON format
```C#
    if (!authenticationService.Authenticate(input.GetAuthorization()))
        return await Task.FromResult(ResponseBuilder.UnAuthorized());
``` 

* BadRequest<T> - Creates a HTTP 400 (BAD_REQUEST) response with a given payload in JSON format
```C#
    var addUserRequest = input.Parse<AddUserRequest>();
    var validation = addUserRequest.ValidateAdd();

    if (!validation.IsValid())
         return addUserRequest.Invalid(validation.Errors).ToJson().BadRequest();
``` 

* BadRequest(this string payload) - Creates a HTTP 400 (BAD_REQUEST) response with a given payload in JSON format
```C#
    return "invalid".BadRequest();
``` 
