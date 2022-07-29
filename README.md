# Project_Omega
 
##Changelog

17/07/22
- Initial commit
- Began work on main menu
	- Added play, options and quit buttons
	- Added audio and graphics options
		- Audio - master, music, dialogue, ambient mixers
		- Graphics - quality, resolution, window mode dropdowns
- Began work on audio system
- Began work on serialisation to save options

18/07/22
- Finished outline of audio and serialisation systems
- Fixed issue with audio sliders not persisting between sessions
- Fixed issue with quality not correctly applying and persisting between sessions
- TODO: 
	- Fix fullscreen mode, currently doesn't apply correctly
	
20/07/22
- Fixed fullscreen mode not applying correctly between sessions
- Added a confirmation screen when adjusting graphics settings
	- TODO: Consider adding "apply" button instead of changing on every dropdown change
	
28/07/22
- Added third person character controller from Unity Starter Assets
- Added day/night cycle
	- TODO: Change horizon colours based on time of day 
	
29/07/22
- Fixed day/night cycle
		- Horizon now uses gradient
		- Moon visible at night and scene now darkens correctly