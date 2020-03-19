Solution requirements:
- .net Core 3.1
- .net Framework 4.8 (only in the CPUMeasurementService project)
AppSettings
The appsettings.json files are not committed to GitHub. The appsettings.Development.json files are examples. Every IP addresses are set to the loopback address.
CPUMeasurementService configuration
If You install the CPUMeasurementService as a Windows Service You have to set a user to run the service and set the user who will run the service.
The service will generate a client_configuration.json file under the C:\Windows\System32 folder with default values.

Disclaimer!
You have to deploy the database by hand! The backend will not create it!
The connection string is in the CPUMeasurementBackend/appsettings.[Development].json file with Windows Auth as default.

The projects are set to lacalhost as default with preset port numbers.
