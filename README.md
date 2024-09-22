**ExecuteMultipleSP**

Can able to perform Insert, Read, Update and Delete SP by providing SPName and and corresponding paramers as key value pair. 
Read SP not needed anyparameter. so it can pass as empty/null.not should be present in the request.

_Serilog_ - User log for logging all informations and errors. log table was auto created to the database specified in the serilog connection string server. additional columns are added using appsetting.json

_CorrelationID_ - Using a guid for ezch transaction of APIs to uniquely identifying and the same is logged in the log table and in the response.

_Sample Request_

{
    "procedureNames": [
        {
            "spName": "sp_CreateUser",
            "parameters": [
                {
                    "UserName": "testuser",
                    "Email": "test@mail.com"
                }
            ]
        }
    ]
}


**Limitation**

Not able to insert/update/delete multiple paramerts at a time.
