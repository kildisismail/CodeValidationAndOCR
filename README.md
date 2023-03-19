<h2>About CodeValidationAndOCR</h2>

CodeValidationAndOCR consists of 2 endpoints.
1. You can generate 8-character unique campaign code with the Codes endpoint. You can check the correctness of the generated code.
2. It produces a text output by parsing the coordinate information for the description and the related description in the Json response with the Ocr endpoint.


<h2>HTTP Methods</h2>

The application will be available on <a href="http://localhost:5001/swagger">localhost:5001</a><br>

<h3>Codes</h3>

<b>GET /generated</b><br>
<b>POST /verified</b><br>

<h3>Ocr</h3>

<b>POST /uploadjson</b><br>
