# Xerris.DotNet.Core.Aws
AWS shared components

This module contains reusable components for C# AWS Lambdas and other AWS resources

##API

### ApiGatewayProxyRequestExtensions
The ApiGatewayProxyRequestExtensions class provides convenience methods for worjking with the APIGatewayProxRequest 

Parse<T> | Deserializes the JSON body into a C# Dto
------- |-----------

```C#
    var addUserRequest = apiRequest.Parse<AddUserRequest>();
```

GetQueryString  |Retrieves the value of a querystring 
----------------|------------------------------------
                      
```C#
    var searchString = apiRequest.GetQueryString("search_string");
```                      
 
GetAuthorization  | Retrieves the Authorization header
----------------- | ------------------------------------------------------------------
```C#
    var searchString = apiRequest.GetAuthorization();
``` 

### ResponseBuilder
Factory method extensions that easily create the APIGatewayResponse 

OK<T>  | A HTTP 200 (OK) response with a JSON payload
-------|---------------------------------------------------------------------
```C#
    var results = await service.GetListByIdAndDescriptionAsync(request.GetQueryString("search_string"));
    return results.Ok();
``` 

Created<T>  | A HTTP 201 (CREATED) response with a JSON payload
------------|--------------------------------------------------
```C#
    var response = await userManagementService.AddUserAsync(addUserRequest).ConfigureAwait(false);
    return response.Created();
``` 

Accepted<T> | A HTTP 202 (ACCEPTED) response with a JSON payload
------------|---------------------------------------------------
```C#
    var response = await userManagementService.DeleteUserAsync(addUserRequest).ConfigureAwait(false);
    return response.Accepted();
``` 

Error(this string error) | A HTTP 500 (SERVER_ERROR) response with a JSON payload
-------------------------|--------------------------------------------------------
```C#
    var response = await userManagementService.DeleteUserAsync(addUserRequest).ConfigureAwait(false);
    return response.Accepted();
``` 

UnAuthorized<T> | A HTTP 400 (BAD_REQUEST) response with a JSON payload
-------------------------|---------------------------------------------
```C#
    if (!authenticationService.Authenticate(input.GetAuthorization()))
        return await Task.FromResult(ResponseBuilder.UnAuthorized());
``` 

BadRequest<T> | A HTTP 400 (BAD_REQUEST) response with JSON payload
-------------------------|-----------------------------------------
```C#
    var addUserRequest = input.Parse<AddUserRequest>();
    var validation = addUserRequest.ValidateAdd();

    if (!validation.IsValid())
         return addUserRequest.Invalid(validation.Errors).ToJson().BadRequest();
``` 

BadRequest(this string payload) | A HTTP 400 (BAD_REQUEST) response with a JSON payload
-------------------------|-------------------------------------------------------------
```C#
    return "invalid".BadRequest();
``` 
## Repositories

###DynamoDb BaseRepository
Used to interact with DynamoDB JSON-based tables

####Method Summary
FindOneAsync<T>(ScanCondition) | Scan for an entity that matches the criteria provided
-------------------------|-----------------------------------------
```C#
    public async Task<User> FindUserAsync(string userName)
    {
        return await FindOneAsync<User>(WhereEquals(nameof(User.Email), userName));
    }
``` 

FindOneAsync<T>(IEnumerable<ScanCondition>) | Scan for an entity that matches the criteria provided
-------------------------|-----------------------------------------
```C#
    //TODO
``` 

SaveAsync() | Saves an Entity to the table the Repository is based on
-------------------------|-------------------------------------------
```C#
    await userRepository.SaveAsync(user);
``` 

DeleteAsync() | Deleted an Entity to the table the Repository is based on
-------------------------|-----------------------------------------------
```C#
    await userRepository.DeleteAsync(user);
``` 

###ScanConditionExtensions
Extensions methods that generate DynamoDB ScanConditions using reflection on domain objects.

####Example Usage
```C#
    var movieStar =  new Foo("Angelina", "Jolie");
    var eq = movieStar.Equals(x => x.FirstName, angelina);
``` 

####Method Summary

Equals(x => x.FirstName, subject) | Equals Condition
-------------------------|-----------------------------------------------
```C#
    var movieStar = new Foo("Angelina", "Jolie");
    var eq = movieStar.Equals(x => x.FirstName, angelina);
``` 

NotEqual(x => x.FirstName, subject) | NotEqual Condition
-------------------------|-----------------------------------------------
```C#
    var movieStar = new Foo("Angelina", "Jolie");
    var eq = movieStar.NotEqual(x => x.FirstName, angelina);
``` 

NotEqual(x => x.FirstName, "Pitt") | NotEqual Condition
-------------------------|-----------------------------------------------
```C#
    var movieStar = new Foo("Angelina", "Jolie");
    var eq = movieStar.NotEqual(x => x.FirstName, "Pitt");
``` 

Contains(x => x.FirstName, subject) | Contains Condition
-------------------------|-----------------------------------------------
```C#
    var movieStar = new Foo("Angelina", "Jolie");
    var eq = movieStar.Contains(x => x.FirstName, angelina);
```

NotContains(x => x.FirstName, subject) | NotContains Condition
-------------------------|-----------------------------------------------
```C#
    var movieStar = new Foo("Angelina", "Jolie");
    var eq = movieStar.NotContains(x => x.FirstName, angelina);
```

GreaterThan(x => x.FirstName, subject) | GreaterThan Condition
-------------------------|-----------------------------------------------
```C#
    var movieStar = new Foo("Angelina", "Jolie") {Age = 44};
    var eq = movieStar.GreaterThan(x => x.Age, movieStar);
```

NotGreaterThan(x => x.FirstName, subject) | NotGreaterThan Condition
-------------------------|-----------------------------------------------
```C#
    var movieStar = new Foo("Angelina", "Jolie") {Age = 44};
    var eq = movieStar.NotGreaterThan(x => x.Age, movieStar);
```

LessThan(x => x.FirstName, subject) | LessThan Condition
-------------------------|-----------------------------------------------
```C#
    var movieStar = new Foo("Angelina", "Jolie") {Age = 44};
    var eq = movieStar.LessThan(x => x.Age, movieStar);
```

LessThanOrEqual(x => x.FirstName, subject) | LessThanOrEqual Condition
-------------------------|-----------------------------------------------
```C#
    var movieStar = new Foo("Angelina", "Jolie") {Age = 44};
    var eq = movieStar.LessThanOrEqual(x => x.Age, movieStar);
```

And(ScanCondition) | A way to aggregate multiple ScanConditions
-------------------------|-----------------------------------------------
```C#
    var movieStar = new Foo("Angelina", "Jolie") {Age = 44};
    var condition = movieStar.Equals(x => x.FirstName, movieStar)
                        .And(movieStar.GreaterThan(x => x.Age, movieStar))
                        .And(movieStar.NotEqual<Foo>(x => x.LastName, "Pitt")).ToList();
```