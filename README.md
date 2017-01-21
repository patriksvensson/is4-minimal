# IdentityServer 4 - Minimal Reproducable Example

* Server: http://localhost:5000
* WebAPI app: http://localhost:7000
* ASP.NET MVC app: http://localhost:8000

*NOTE: For the sake of making a minimal reproducable example, I've
greatly simplified the setup.*

## What currently works

The WebAPI app is partitioned into two API:s, an internal API 
(only available to internal consumers) and a third party API
that's available to third party API consumers.

The ASP.NET MVC app will only be accessible to administrators. 
The site itself will call the WebAPI on behalf of the logged in user.

For this I've declared three different clients in IdentityServer 
that can be found in `MyIdentityServer/Configuration/Clients.cs`.

* `client.internal` (*client_credentials*)
  * scope: `api.internal`
  * scope: `api.thirdparty`

* `client.thirdparty` (*client_credentials*)
  * scope: `api.thirdparty`

* `openidconnect` (*hybrid*, *client_credentials*)
  * scope: `api.internal`
  * scope: `api.thirdparty`

I've then in my WebAPI application setup authorization policies
to limit API consumers to the different APIs. Internal API consumers
will have access to `api.internal` and `api.thirdparty` while 
third party API consumers will have access to only `api.thirdparty`.

```csharp
services.AddAuthorization(options =>
{
    // For Authorize("internal")
    options.AddPolicy("internal", policy =>
    {
        policy.RequireClaim("scope", "api.internal");
    });

    // For Authorize("thirdparty")
    options.AddPolicy("thirdparty", policy =>
    {
        policy.RequireAssertion(context => 
            context.User.HasClaim("scope", "api.internal") ||
            context.User.HasClaim("scope", "api.thirdparty"));
    });
});
```

Everything works as expected here.

## Questions

1. Is this the way to scope the different API:s or have I misunderstood something?
2. Do you see something strange in this setup?
