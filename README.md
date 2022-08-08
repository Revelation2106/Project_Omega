# Project_Omega
 
##Changelog

08/08/22
- Refactored AudioManager to use ManagedInstance
- Fixed bug with reverting graphics changes not setting window mode correctly

07/08/22
- Refactored event system
	- Now uses enums for event types instead of classes
	- Option for data within events
	- Added PostNow function to prevent issues with execution order
- Refactored dialogue system to use new event system
- Refactored input system to use new event system

02/08/22
- Continued work on dialogue system
	- Fixed issue with dialogue skipping due to race condition
	- Added name and portrait/portrait position support
- TODO: Make dialogue serialisable
	- Need to save position in dialogue per Ink file and when loading dialouge
	  don't pass in new Story()
	  
01/08/22
- Began work on dialogue system
	- TODO: 
		- Fix issue with dialogue skipping
		- Make dialogue serialisable
		- Add name and portrait tags to Ink files and integrate into UI
	  
29/07/22
- Fixed day/night cycle
		- Horizon now uses gradient
		- Moon visible at night and scene now darkens correctly

28/07/22
- Added third person character controller from Unity Starter Assets
- Added day/night cycle
	- TODO: Change horizon colours based on time of day 

20/07/22
- Fixed fullscreen mode not applying correctly between sessions
- Added a confirmation screen when adjusting graphics settings
	- TODO: Consider adding "apply" button instead of changing on every dropdown change

18/07/22
- Finished outline of audio and serialisation systems
- Fixed issue with audio sliders not persisting between sessions
- Fixed issue with quality not correctly applying and persisting between sessions
- TODO: 
	- Fix fullscreen mode, currently doesn't apply correctly

17/07/22
- Initial commit
- Began work on main menu
	- Added play, options and quit buttons
	- Added audio and graphics options
		- Audio - master, music, dialogue, ambient mixers
		- Graphics - quality, resolution, window mode dropdowns
- Began work on audio system
- Began work on serialisation to save options