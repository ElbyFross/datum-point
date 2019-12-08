## What the mission?
That plugin provides fetures focused on statisics collecting and API for analysis and work of stored data.

## Main conceptions and rules
### Distributed coputing
Application targeting focused on poor infrastructure state of target customer. According to this declaring developer can't account 
that customer will has sever that work 24/7 and has enought performance to compute statistic data for all users without demage to
processing of base tasks.

Regards to this we can suppose that would be much more effective to use a network based on few low performance client's terminals 
to operate with data that placing all computing on server.

#### AC must be based on rules:
- One task sending to few clients, to prevent data coruption.
	- If count of clients less than required minimum during long enough period then server start computing by it self,	
	concluding that it has enough processor time for that.
- Any raw data can't be removed from data storage for case if administration conclude that 
high end data was corrupted by bug, malware or cheting of few client's.
- Data is anonymous. 
	- Server sharing only the tasks without providing information about source and target.
	- Server not provide information about scaling.
- Task must be done in provided quant. 
	- After quant would be closed influence on that task not possible.
	- The fact that some of client's not return the answers in not metter.
- If answers less than enough to determine what is answers correct, then data must be droped.
- Client should be able to disable computing for prevent damaging of performance on too week machines.
	- Client should be able to select how many tasks it could compute per one mement.
	- Automaticly system set the count of tasks like count of logical cores minus one.
- Computing possible only if server's analysis plugin and client's has the same versions.
	- Client should be able to request assembly from server as downloadable dll, to prevent requirements	
	of manual updating from every machine in infrastructure.

### Data scale
After passing of some time period data must be recomputed to huger one, to prevent too detailed computing each time.
There is no necessary to know and recompute every time statistic for previous years and compare querent one with it.

#### Data work by the sheme:
`last 30 days` + `days of the previous month's blok until possibility to close it` => `months that can be grouped to quarter` 
=> `quarter that can't be grouped to year' => `years`

#### Data blocks example: 
`30 days + 17 days from 31 days for next unclosed month => previous 3 monthes => 2 quarters => 3 years`

### Uniform data computing
Clients and server has the same data analysis libraries that simplify computing instructions and provide simplifying of computing.

#### Hight end alorithm of call
1. Client asking server for data
2. Server trying to reach data
	1. Server try to get data by internal alorithm of IQueryHandler
	2. If data not found then to server's answer will contains attached task instruction that would allow precompute data before
	continue query handling.
	3. Server place data to the tasks sheduler.

#### Server request:
- Assembly version.
- Operation GUID.
- Binary data block inheired from uniform interface.
- List of target operations in format of full methods' names.

#### Client's answer:
- Operation GUID.
- Binary answers in format `OperationName` => `Answer`


#### IOperation
Interface that provide possibility to proceed data.
- Compute(string guid, object data) - method that will has been calling fro operation computing.

### Data's types
#### DB concept
Is core is User or Day?

User 
Pluses:

### Analytics properties
- Assessment - assessment of this session. In case of not atomar session it would be average value.
- AssessmentsDelta - difference between highest and lowes assessment during session. Zero for atomar session.
- AbcencesDayScaleMask - mask that show behevior in skiping of sessions frequency of sessions leveng during one period. 
Example: 16+7+3+3+2+4+9+14 - this mask show that student has strong tend to skip first and last two lessons.
- AbcencesWeekScaleMask - mask that show behevior in skiping of sessions frequency of sessions leveng during one period. 
Example: 3+1+2+3+4+8+0+0 - The mask of absence relative to week's days. It showing that student has tend to absent at friday.

