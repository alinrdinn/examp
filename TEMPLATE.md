# Common API Documentation

## Authentication

All endpoints require `Bearer` token authorization. Include the following header in your requests:&#x20;

```http
Authorization: Bearer <token>
```

## Base Response Structure

All successful responses follow this structure:&#x20;

```json
{
  "code": 200,
  "success": true,
  "data": {
    // Endpoint-specific data structure
  }
}
```

# {Module Name} API Documentation

## Overview

The {Module Name} provides APIs for {what this module does in one sentence}. All endpoints require authentication and are prefixed with `/v1/{module-slug}`. (Replace `{module-slug}` with the module’s URL prefix used by your service.)&#x20;

## Endpoints

> Duplicate the following endpoint blocks for every endpoint in this module. Keep headings and ordering exactly as shown below.

### 1. {Endpoint Title}

**Endpoint:** `{HTTP_METHOD} /v1/{module-slug}/{path-fragment}`

**Description:** {One–two sentences that clearly state what this endpoint returns/does and any notable behaviors (e.g., aggregation, filtering, export).}

**Database Functions/Tables Used:**

* {List every DB function/table used by this endpoint.}
* `{schema.function_or_proc_name(param1, param2, ...)}` — {short purpose of the function (e.g., “main data retrieval”, “aggregated metrics”)}
* `{schema.table_name}` — {what it’s used for (e.g., “time dimension”, “lookup”, “join source”)}
* {Add as many bullets as needed; include every function/table touched by the endpoint’s logic.}&#x20;

**Parameters:**

* {Only if needed, e.g., placeholders for dynamic function switches.}
* {Document any symbolic parameters used in dynamic function names or path fragments, e.g., `{category}` = "productivity" | "quality"; `{granularityData}` = "daily" | "weekly" | "monthly". Only include if applicable.}&#x20;

**Request Body:**
Format (match the request class for this endpoint; add inline comments describing format, allowed values, and examples):

```json
{
  // Example scaffold — replace fields to reflect the endpoint's request class
  // Define fields per this endpoint’s request class;
  // include allowed values, formats (e.g., yyyy, yyyyqq, yyyymm, yyyyww, yyyyMMdd), and constraints as comment.
  "firstField": {
    "firstSubField": string, // allowed: "..." | "..." | "..." ...
    "secondSubField": string   // exact location name; case-sensitive if applicable
  },
  "secondField": {
    ...
  },
  "thirdField": string,
  "fourthField": number,
  ...
}
```

**Response:**
Format (match the endpoint’s response class; include full shape and inline field commentary):

```json
{
  "code": number,   // HTTP-like status code mirrored in body
  "success": boolean,
  "data": {
	  // Example scaffold — replace fields to reflect the endpoint's response class
	  // Define fields per this endpoint’s response class;
	  // include line description for each field e.g.,:
	  // "data": {
      //   "y": array[string], // labels along one axis (e.g., time buckets)
      //   "xs": [
      //     {
      //       "type": string,   // e.g., "line" (include if applicable)
      //       "label": string,  // e.g., "actual", "threshold", "projection"
      //       "x": array[number|null] // data points aligned to "y"
      //     }
      //   ]
      // }
	  ...
	},
  "errors": object|null // include error payload schema if standardized
}
```

---

### 2. Export {Endpoint Title} to {File Type} (if applicable)

**Endpoint:** `POST /v1/{module-slug}`

**Description:** Exports {data type} to Excel format (.xlsx file).

**Database Functions/Tables Used:**

* {Function(s) used for export data pulling}
* {Auxiliary functions/tables used for ranges/labels (e.g., date-range generator, dim tables)}

**Request Body:**
Format:

```json
{
  // Include all filtering fields (location, time, domain object, etc.) with formats and allowed values.
}
```

**Response:**
Format:

```http
Content-Type: {file_type}
Content-Disposition: attachment; filename="{suggested-file-name}.{ext}"
```

Binary file download.&#x20;

---

... Other Endpoints or Modules

## Error Handling

List the standardized errors that this module can return. Keep names and wording consistent with the existing style; for example:

1. **Data Not Found** — when no data exists for the provided filters.
2. **Invalid Time Parameters** — when `period`/`value`/`granularity` combinations are invalid.
3. **Invalid Location** — when `location.level`/`location.name` is unrecognized.
4. **Invalid {Domain Object}** — when `{domainObject}.name` is unsupported.&#x20;

---

# Database Functions Reference

This section provides detailed information about the database functions used by this module. Use one subsection per function and document parameters, types, and return shape precisely.&#x20;

## {Module Name} Functions

### `{schema.function_name}`

**Parameters:**

* `{param}` ({type}): {allowed values / format hints}
* {List all parameters with types and allowed values}

**Returns:** {Concise description of what the function returns (e.g., “chart series with actual and projection values”, “range-based aggregated metrics”).}&#x20;

### `{schema.function_name_2}`

**Parameters:**

* {params…}

**Returns:** {Description.}

> Add as many function entries as needed. If function names are dynamic (e.g., include `{category}` or `{granularityData}` tokens), enumerate the variations and what they resolve to.&#x20;

## Database Tables

List every table the module touches, with a short description and key columns that matter for joins/filters/labels.

### `{schema.table_name}`

**Description:** {What the table represents.}
**Key Columns:** `{col_a}`, `{col_b}`, `{col_c}` — {what each is used for in this module}
**Usage:** {How this table is used (e.g., “time range calculations”, “dimension join”, “lookup”)}&#x20;