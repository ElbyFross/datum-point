### Idea
- Master might not note changes in young padavan's behavior.
This plugin must help by highlighting ones who falling in dark side of power or reaching success following jedi path.
	- Abnormally changing of assessments to decrease side can inform about problem in student's life. Maybe he need some help.
	- If student have increased\decreased his assessments in short period it need to be mentioned in personal conversation to motivate movement.
	- Important part is focusing on weekends affecting to activity. 
	Critical decrease of results after weekend could mean child abusing or other serious problems in family.
	If child are often disappear after weekend without information about this could mean about violence.
- Masters are also humans. This tend to creating of 'favorites' and 'rouges'. Application need to remind that this not good practice.
	- Detecting common rate of assessments to inform teacher about focusing on certain persons but not on all class.
	- Detecting abnormally low success of certain person in subject against to stastisic in other subjects can mean that teacher hate this student.
	Such situation need to be shared to department head for receiving conclusion about this employee and situation.

#####Jokes away. 
That's plugin can improve the future of students and save someones from failed life.
Possible effect is lower crime statistic, higher economics level, moving science forward.
Destiny of human mostly selecting by environment in childhood.

### Modules
#### Analytics - scalable coollection of scripts with integration to sheduler, that provide data monitoring.
- AbnormallyWeakSubject - script that detect an abnormally low success of student in subject relative to success in others.
- SubjectClassActivity - script that detect if class has some significant less active students.
- AssessmentsTrend - scipt that monitor common course of student's essesments and inform about changes and abnormally peaks.

### Processing
#### Highend algorithm
1. Client react on action and place analisys to server's task shedule.
2. Server making analisys.
3. If server detect abnormal situation it place event handler to user's advices journal.
4. Authorized users via client can select how to react on this event.
5. If situation ignored server can share warning to superiors and coworkers.

#### Protection levels
1. Normal level. 
	
	Base frequency of profile analisys.

2. Low protection level. 

	Caused by: 
	- Abnormal user's statistic.

	Reaction: 
	- Informate nearst authority about changes in behavior.

3. Middle protection level. 
	
	Caused by: 
	- Behavior not has detected reasons, but not had confirmation of the problem.

	Reaction:
	- Server deciding that this can be violence and inform about that not only a classroom teacher but also psychologist and head teacher users.
	- Server spawn notification in child's terminal that family violence is unacceptable and inform about authorized users that can help. 
	- Informs parents' accaunts about problem and warn about consequences of child abussing or domestic violence. 
	Recommending to talk with child about bullying or mental healthiness.

4. High protection level.

	Caused by: 
	- Athorities answer that all in normal without confirming the reasons, such as sickness.

	Reaction:
	- Prolonging protection time.
	- Informs user that server warring about this situation. 
	- Surveying user about causes of lowering assessments\activity\absent.
	- Informs authority in case if survey show the problem.
	- Activate "HELP ME" banner in user's profile that immediately inform every authority user after request.

	Analisys algorithm changes:
	- Analisys use short time statistic  during last month to detect peaks. 
	If detected decreasing then autorize Level 5.

5. Forced protection level.

	Caused by:
	- Authority confirm domestic violence\abussing\bullying.
	- System detect problems on 4th level.

	Reaction:
	- Server informing previous authorities about considering that there possible dangerous situation.
	- Activate warning banner for every authority user familiar to user.
	- Informs social service's account about .
	- Informs parents' accaunts about highest prtection level. Provide contacts of from social service's account.
	
	Analisys algorithm changes:
	- Analysis disabled and just collect statistic with high frequency.
	- Level can be decreased only by social service's account.


#### Possible users' stories
##### Story #1
- Emily has a positive essestments, but after hollidays she was absent during few days, 
and after this his essesmsnts start drop down.
- Server detect abnormaly situation. 
- Server increase protection to 2th level.
	- It asking classroom teacher to talk with Emily.
- Emily didn't say anithig bad about weekend and seem ok but sickness not confirmed as reason. 
- Teacher conclude that child just too relaxed on the weekend and mark that in the answer to analytic script.
- Server increase protection level for this child for the nearest 2 months cause there is a risk that child can be silent about problems.
- After two weeks Emily also skip a day after weekend. Essesments level not return to previous trend.
- Server increase protection to 3th level and update security protection up to 4 months.	
- Server get answers from all autority that all is ok. (In case of confirmation increase protection level to 5th level)
- Increase protection level to 4th level. 
- Registred abnormal situation based on rule of 4th protection level.
- Increase protection level to 5th level. 
