WebApiAsyncETagStrategy
=======================

An async pattern for reading and setting ETag headers in ASP.NET Web API.

# Goals

- Single file like PetaPoco / Massive
- ETag caching only, at least at first
- Way to specify
  - ETag Lookup Function
  - Model Get Function
  - Property from Model with current ETag
- Take care of checking the Request Headers and setting the Response Headers
