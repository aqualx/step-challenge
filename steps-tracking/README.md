# StepsTracking API

## Overview

StepsTracking API is a .NET Core 9 backend application for a company-wide steps leaderboard challenge for teams of employees.

The API supports:

- **Teams**: Create/View/Delete
- **Users**: Create & Assign to a team/View
- **Counters**: Create/View/Delete
- **Authorization**: Generate user token for accessing protected endpoints: Team delete, Add counter for user, Delete counter for user

## Features

- User authentication and login
- User and team management
- Step tracking with counter increments
- Retrieval of user and team step statistics
- Secure deletion of teams and counters

## Technologies Used

- **.NET Core 9**
- **ASP.NET Web API**
- **EF Core** for DB abstraction
- **SQLite** for persistant storage layer
- **OpenAPI Specification** + **[Scalar](https://github.com/ScalaR/ScalaR)**
- **Apidog** for playing with endpoints
- TODO: Add data caching via HybridCache package [ZiggyCreatures.FusionCache](https://github.com/ZiggyCreatures/FusionCache) that allows configuring multi-level cache (L1 memory + L2 distributed)

## API Documentation

The API is documented using OpenAPI and can be accessed via:

- Scalar: <https://steps-tracking-challenge.azurewebsites.net/scalar/v1>
- Apidog documentation: <https://www.apidog.com/apidoc/shared-7410b38e-5e31-4b71-b2ee-a9154dc94ea9/api-14168288>

## Endpoints

### Authentication

- `POST /login` - User login.\
Response:
<pre>
{
    "token": "eyJhbGciOiJIUzI1Ni...."
}
</pre>

### User Management

- `POST /users` - Create a new user. TODO: Protect with TOKEN of user with admin rights.\
Response:
<pre>
{
    "id": "0195259c-d33e-7201-9cc8-244d6a87a559",
    "name": "New user 3",
    "teamId": "3f4aca6a-018d-4089-8f3d-73bd16a348ed"
}
</pre>
- `GET /users` - Get all users.\
Response:
<pre>
[
    {
        "id": "c6052821-5051-4c8d-a479-e56de111a75e",
        "name": "AstralEdge",
        "teamId": "374a69cc-ca99-4e6e-9bfb-78aaa77b1936"
    },
    {
        "id": "01952304-cc4c-713a-979d-a203818af0f6",
        "name": "WindWhisperer",
        "teamId": "3f4aca6a-018d-4089-8f3d-73bd16a348ed"
    }
    ...
]
</pre>
- `GET /users/{userId}` - Get user by ID.\
Response:
<pre>
{
    "id": "c6052821-5051-4c8d-a479-e56de111a75e",
    "name": "AstralEdge",
    "teamId": "374a69cc-ca99-4e6e-9bfb-78aaa77b1936"
}
</pre>

### Team Management

- `POST /teams` - Create a new team.\
Response:
<pre>
{
    "id": "01952580-0d9f-71af-8dd9-30580ea6c004",
    "name": "New Team",
    "users": null
}
</pre>
- `GET /teams` - Get all teams.\
Response:
<pre>
[
    {
        "id": "01952580-0d9f-71af-8dd9-30580ea6c004",
        "name": "New Team",
        "users": []
    },
    {
        "id": "374a69cc-ca99-4e6e-9bfb-78aaa77b1936",
        "name": "Rampage Riders",
        "users": [
            {
                "id": "c6052821-5051-4c8d-a479-e56de111a75e",
                "name": "AstralEdge",
                "teamId": "374a69cc-ca99-4e6e-9bfb-78aaa77b1936"
            }
        ]
    },
    ...
]
</pre>
- `DELETE /teams/{teamId}` - Delete a team Note: Protected access. Provide "-H "Authorization: Bearer USER_TOKEN". User with admin rights only.\
Response:
<pre>
Status code: 204
</pre>

### Step Counters

- `POST /counters/increment` - Increment step count for a user. Note: Protected access. Provide "-H "Authorization: Bearer USER_TOKEN".\
Response:
<pre>
{
    "id": "019525a7-2363-7630-8649-a9070b252399",
    "userId": "c6052821-5051-4c8d-a479-e56de111a75e",
    "steps": 99,
    "updatedAt": "2025-02-20T23:17:45.4388088+00:00"
}
</pre>
- `GET /counters` - Get all counters.\
Response:
<pre>
{
    "total": 123,
    "counters": [
        {
            "id": "01951ee1-93cc-77cf-807c-dde25b18c954",
            "userId": "c6052821-5051-4c8d-a479-e56de111a75e",
            "steps": 1,
            "updatedAt": "2025-02-19T16:44:09.9769287+01:00"
        },
        ...
    ]
}
</pre>
- `GET /counters/team/{teamId}` - Get step counters for a team.\
Response:
<pre>
{
    "total": 123,
    "counters": [
        {
            "userId": "c6052821-5051-4c8d-a479-e56de111a75e",
            "steps": 61,
            "lastUpdate": "2025-02-20T23:17:45.4388088+00:00"
        },
        ...
    ]
}
</pre>
- `GET /counters/user/{userId}` - Get step counters for a user.\
Response:
<pre>
{
    "total": 123,
    "counters": [
        {
            "id": "01951ee1-93cc-77cf-807c-dde25b18c954",
            "userId": "c6052821-5051-4c8d-a479-e56de111a75e",
            "steps": 1,
            "updatedAt": "2025-02-19T16:44:09.9769287+01:00"
        },
        {
            "id": "01952029-cbfb-73be-b75c-e0922ba2bc71",
            "userId": "c6052821-5051-4c8d-a479-e56de111a75e",
            "steps": 10,
            "updatedAt": "2025-02-19T22:42:44.8564581+01:00"
        },
        ...
    ]
}
</pre>
- `DELETE /counters/{counterId}` - Delete a step counter. Protected access (owner or admin only).\
Response:
<pre>
Status code: 204
</pre>
