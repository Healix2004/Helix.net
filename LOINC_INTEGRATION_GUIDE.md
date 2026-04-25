# LOINC FHIR Integration - Implementation Guide

## Overview
The LOINC Terminology Service has been successfully integrated into your Helix application. This integration enables you to:
- Look up specific LOINC codes and retrieve their details
- Search for LOINC codes based on search terms
- Create FHIR Observation resources from LOINC codes

## Files Created/Modified

### New Files
1. **Helix.Service\Interfaces\ILoincTerminologyService.cs**
   - Interface defining the contract for LOINC service operations

2. **Helix.Service\Services\LoincTerminology\LoincTerminologyService.cs**
   - Implementation of LOINC terminology service
   - Handles communication with FHIR LOINC API
   - Parses FHIR responses into usable formats

3. **HelixAPI\Controllers\LoincController.cs**
   - REST API endpoints for LOINC operations
   - Endpoints:
     - `GET /api/loinc/lookup/{code}` - Lookup specific LOINC code
     - `GET /api/loinc/search?searchTerm=glucose` - Search for LOINC codes
     - `GET /api/loinc/observation?code=2345-7&display=Glucose` - Get FHIR Observation template

### Modified Files
1. **Helix.Service\ModuleServiceDependancies.cs**
   - Added LOINC service registration in DI container
   - Configured HttpClient with FHIR LOINC API base address and Basic Auth

2. **HelixAPI\appsettings.json**
   - Added LoincApi configuration section

## Configuration

### Required Setup

1. **Update appsettings.json** with your LOINC API credentials:
```json
"LoincApi": {
    "Username": "your_actual_username",
    "Password": "your_actual_password"
}
```

> ⚠️ **Security Best Practice**: For production, use environment variables or a secrets manager instead of hardcoding credentials.

### For Production Environment
Set environment variables or use User Secrets:
```bash
set LoincApi__Username=your_username
set LoincApi__Password=your_password
```

Or use .NET User Secrets Manager:
```bash
dotnet user-secrets set "LoincApi:Username" "your_username"
dotnet user-secrets set "LoincApi:Password" "your_password"
```

## API Endpoints

All endpoints require authentication (Bearer token).

### 1. Lookup LOINC Code
**Endpoint:** `GET /api/loinc/lookup/{code}`

**Example:**
```bash
curl -H "Authorization: Bearer {token}" \
  https://your-api.com/api/loinc/lookup/2345-7
```

**Response:**
```json
{
    "statusCode": 200,
    "succeeded": true,
    "message": "LOINC code retrieved successfully.",
    "data": {
        "code": "2345-7",
        "display": "Glucose [Moles/volume] in Serum or Plasma",
        "system": "http://loinc.org",
        "text": "Glucose [Moles/volume] in Serum or Plasma"
    }
}
```

### 2. Search LOINC Codes
**Endpoint:** `GET /api/loinc/search?searchTerm=glucose`

**Example:**
```bash
curl -H "Authorization: Bearer {token}" \
  "https://your-api.com/api/loinc/search?searchTerm=glucose"
```

**Response:**
```json
{
    "statusCode": 200,
    "succeeded": true,
    "message": "Found 15 LOINC codes matching 'glucose'.",
    "data": [
        {
            "code": "2345-7",
            "display": "Glucose [Moles/volume] in Serum or Plasma",
            "system": "http://loinc.org",
            "text": "Glucose [Moles/volume] in Serum or Plasma"
        },
        {
            "code": "2341-6",
            "display": "Glucose [Moles/volume] in Plasma",
            "system": "http://loinc.org",
            "text": "Glucose [Moles/volume] in Plasma"
        }
    ]
}
```

### 3. Get FHIR Observation Template
**Endpoint:** `GET /api/loinc/observation?code=2345-7&display=Glucose`

**Example:**
```bash
curl -H "Authorization: Bearer {token}" \
  "https://your-api.com/api/loinc/observation?code=2345-7&display=Glucose"
```

**Response:**
```json
{
    "statusCode": 200,
    "succeeded": true,
    "message": "FHIR Observation template created successfully.",
    "data": {
        "resourceType": "Observation",
        "status": "final",
        "category": [
            {
                "coding": [
                    {
                        "system": "http://terminology.hl7.org/CodeSystem/observation-category",
                        "code": "laboratory",
                        "display": "Laboratory"
                    }
                ]
            }
        ],
        "code": {
            "coding": [
                {
                    "system": "http://loinc.org",
                    "code": "2345-7",
                    "display": "Glucose"
                }
            ],
            "text": "Glucose"
        },
        "effective": "2025-02-20T12:34:56Z",
        "issued": "2025-02-20T12:34:56Z"
    }
}
```

## Usage Examples

### C# Code Examples

#### Lookup LOINC Code
```csharp
using Helix.Service.Interfaces;

public class MyController : ControllerBase
{
    private readonly ILoincTerminologyService _loincService;

    public MyController(ILoincTerminologyService loincService)
    {
        _loincService = loincService;
    }

    public async Task DoSomething()
    {
        var concept = await _loincService.LookupLoincCodeAsync("2345-7");
        var code = concept?.Coding?.FirstOrDefault()?.Code;
        var display = concept?.Coding?.FirstOrDefault()?.Display;
    }
}
```

#### Search LOINC Codes
```csharp
var codes = await _loincService.SearchLoincCodesAsync("glucose");
foreach (var code in codes)
{
    Console.WriteLine($"{code.Coding.FirstOrDefault()?.Code}: {code.Coding.FirstOrDefault()?.Display}");
}
```

#### Create FHIR Observation
```csharp
var observation = _loincService.CreateFhirObservation("2345-7", "Glucose");
// Use observation.Coding, observation.Category, etc.
```

## Error Handling

The service includes proper error handling:

| Error | Status Code | Reason |
|-------|------------|--------|
| Missing LOINC code | 400 | Required parameter not provided |
| Code not found | 404 | LOINC code does not exist |
| Invalid search term | 400 | Search term is required |
| Authentication failed | 401 | Invalid LOINC API credentials |
| Network error | 400 | Cannot reach FHIR LOINC API |

### Handle Errors in Your Code
```csharp
try
{
    var concept = await _loincService.LookupLoincCodeAsync(code);
}
catch (InvalidOperationException ex)
{
    // Log the error
    logger.LogError($"LOINC lookup failed: {ex.Message}");
    // Return appropriate HTTP response
    return BadRequest(new Response<string> { Message = ex.Message });
}
```

## Troubleshooting

### Issue: 401 Unauthorized
**Cause:** Invalid LOINC API credentials
**Solution:** 
1. Verify credentials in appsettings.json
2. Check that credentials are not expired
3. Contact LOINC support for credential verification

### Issue: No results found
**Cause:** Search term too specific or LOINC code doesn't exist
**Solution:**
1. Try broader search terms
2. Verify LOINC code format
3. Check LOINC website directly: https://loinc.org

### Issue: Timeout or network errors
**Cause:** FHIR LOINC API unreachable or slow
**Solution:**
1. Check internet connectivity
2. Verify firewall rules allow access to fhir.loinc.org
3. Check LOINC API status page
4. Implement retry logic with exponential backoff

## Testing

### Using Swagger UI
1. Start the application
2. Navigate to `/swagger`
3. Scroll to "Loinc" section
4. Expand endpoints and click "Try it out"
5. Enter test values (e.g., code: "2345-7")
6. Click "Execute"

### Using Postman
1. Create new request: `GET /api/loinc/lookup/2345-7`
2. Add header: `Authorization: Bearer {your_token}`
3. Send request

### Using cURL
```bash
curl -X GET "https://your-api.com/api/loinc/lookup/2345-7" \
  -H "Authorization: Bearer your_token" \
  -H "Content-Type: application/json"
```

## Performance Considerations

1. **Caching**: Consider implementing caching for frequently searched codes
2. **Rate Limiting**: LOINC API may have rate limits; implement request throttling
3. **Async/Await**: All service methods are async; use them properly in controllers
4. **Connection Pooling**: HttpClient is registered once in DI; reuse it (already done)

## Future Enhancements

1. Add caching layer for frequently accessed codes
2. Implement pagination for search results
3. Add batch lookup capability
4. Create a local LOINC database cache for faster responses
5. Add support for other terminology standards (SNOMED CT, RxNorm)

## References

- [LOINC Official Website](https://loinc.org)
- [LOINC FHIR Integration](https://loinc.org/fhir/)
- [HL7 FHIR Specification](https://www.hl7.org/fhir/)
- [Your Application Documentation](../README.md)

## Support

For issues or questions:
1. Check this documentation
2. Review error logs
3. Consult LOINC API documentation
4. Contact development team
