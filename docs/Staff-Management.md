#### <u>Domain Models</u>
`Role` : 
* Name: string, 
* `ReportsTo`: Role  

`Employee` : 
* First name, 
* Last name, 
* Email Address, 
* Date of birth, 
* Hiring date, 
* Termination date, 
* `Role`: Role, 
* `Manager`: Employee  
 
#### <u>Use Cases</u>
| Use Cases             | Description                | Actors        | Events   | Events Subscribers |
|:----------------------|:---------------------------|:--------------|:---------|:-------------------|
| CRUD on Roles         | CRUD on Roles              | Administrator | None     | None               |
| Create an employee    | Creates a new employee     | Administrator | None     | None               |
| Update an employee    | Updates an employee's info | Administrator | None     | None               |
| Terminate an employee | Terminates an employee     | Administrator | None     | None               |
