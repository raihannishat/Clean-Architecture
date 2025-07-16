# Dispatcher API OpenAPI Usage Guide

This document explains how to use the OpenAPI (Swagger) specification for the Dispatcher endpoint in your project.

## 1. Save the OpenAPI YAML

Copy the OpenAPI YAML specification (see below) and save it as a file, for example:

```
dispatcher-openapi.yaml
```

## 2. Import into Swagger UI

- Go to [Swagger Editor](https://editor.swagger.io/)
- Paste the YAML content into the left panel, or use `File > Import File` to upload your YAML file.
- The right panel will show the API documentation and "Try it out" options.

## 3. Import into Postman

- Open Postman
- Go to `File > Import > File`
- Select your `dispatcher-openapi.yaml` file
- Postman will generate a collection with the Dispatcher endpoint, ready to use.

## 4. Import into Other OpenAPI Tools

- Use the "Import/Open/OpenAPI/Swagger" option in your tool (e.g., Insomnia, Stoplight, Redocly)
- Select the YAML file
- The tool will parse the API and provide documentation and testing features.

## 5. Example OpenAPI YAML

Below is the full OpenAPI YAML for the Dispatcher endpoint:

```yaml
openapi: 3.0.3
info:
  title: Dispatcher API
  version: 1.0.0
  description: |
    A dynamic endpoint for dispatching CQRS commands and queries.
    - `OperationName` should match the base name of a Command or Query (without the suffix).
    - `Data` should be a JSON string representing the payload for the operation.

paths:
  /api/dispatcher:
    post:
      summary: Dispatch a command or query
      description: |
        Dynamically dispatches a CQRS command or query based on the provided operation name and payload.
        The operation is resolved by convention: if the name starts with "Get", it is treated as a Query; otherwise, as a Command.
      tags:
        - Dispatcher
      requestBody:
        required: true
        content:
          application/json:
            schema:
              $ref: '#/components/schemas/DispatcherRequest'
            examples:
              GetBlogPosts:
                summary: Get blog posts (query)
                value:
                  OperationName: GetBlogPosts
                  Data: '{"Page":1,"PageSize":10}'
              Register:
                summary: Register a user (command)
                value:
                  OperationName: Register
                  Data: '{"FirstName":"John","LastName":"Doe","Email":"john@example.com","UserName":"johndoe","Password":"pass123","ConfirmPassword":"pass123"}'
      responses:
        '200':
          description: Success
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/DispatcherResponse'
              examples:
                Success:
                  summary: Successful operation
                  value:
                    success: true
                    message: Operation completed successfully
                    data: { }
                    errors: null
                    statusCode: 200
        '400':
          description: Bad Request (validation or deserialization error)
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/DispatcherResponse'
              examples:
                ValidationError:
                  summary: Validation failed
                  value:
                    success: false
                    message: Validation failed
                    data: [ "Page must be greater than 0" ]
                    errors: null
                    statusCode: 400
        '404':
          description: Operation not found
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/DispatcherResponse'
              examples:
                NotFound:
                  summary: Operation not found
                  value:
                    success: false
                    message: Operation 'SomeOperation' not found
                    data: null
                    errors: null
                    statusCode: 404
        '500':
          description: Internal server error
          content:
            application/json:
              schema:
                $ref: '#/components/schemas/DispatcherResponse'
              examples:
                ServerError:
                  summary: Internal error
                  value:
                    success: false
                    message: Internal server error: <error details>
                    data: null
                    errors: null
                    statusCode: 500

components:
  schemas:
    DispatcherRequest:
      type: object
      required:
        - OperationName
        - Data
      properties:
        OperationName:
          type: string
          description: The base name of the operation (e.g., "GetBlogPosts", "Register").
        Data:
          type: string
          description: JSON string representing the payload for the operation.
    DispatcherResponse:
      type: object
      properties:
        success:
          type: boolean
        message:
          type: string
        data:
          description: The result of the operation (type varies by operation).
        errors:
          type: array
          items:
            type: string
          nullable: true
        statusCode:
          type: integer
          format: int32
``` 