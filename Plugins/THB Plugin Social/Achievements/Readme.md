### Idea
- Implement entertainment in educational process.
- Provide short-time goals:
- Implement competitive atmosphere by rules:
	1. Motivate to compete with nearby by level classmates. Not with top leader that has to huge gap.
	2. Avoid top table, that can't motivate to move forward ones who further than 1-2 page. 

### Modules
#### WPF - provide achivment visualization in WPF.
- AchievementUIControoller - provide UI item that could display achievement data.

#### Controller
- AchievementContainer - data container that provide achievement's data.
	- MetaData - code, default data, state, level
	- Executable - code to check condition.
	- Resources - icon, custom levels' borders
	- Localization - collection of multi language dictionaries that provide localization of title and description.
- API - common methods to achievement's managment.
	- LoadCollection - loading achievement containers to application.
	- ValidateOnClient - validation algotirthm optimised for clients.
	- ValidateOnServer - validation algotirthm optimised for servers.
- Queries
	- GetAchievementsList - return achievement's metadata.
	- SetAchievementState - send new achievement to validation.

### Processing
1. Client load local data
2. Client analyse data and coclude does user have new achievement.
3. Client informs server about new achievements.
4. Server add achievement's validation to schedule.
5. Server validate achievement and conclude is it deserved?
