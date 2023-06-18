# Controls
- W A S D  to simple move the player.
- press space to Jump.
- Player will make loud noise on every Jump.

# Step To Assign WayPoints
- Simply create a scirptible WayPoint Object in the assets/WayPoints Folder
- Assign that object to the waypoints array in the WaypointManager
- The WaypointManager is assign to a child of an empty object of Managers

# Guard AI

Guard AI is a script that controls the behavior of a guard character in a game scenario. It enables the guard to patrol between waypoints, detect the player, and respond to player noise.

## Features

- Waypoint Patrolling: The guard moves between predefined waypoints in a patrol route, smoothly transitioning between them.
- Player Detection: The guard's spotlight detects the player within a specified range and angle, and initiates pursuit.
- Player Chase: The guard chases the player, attempting to reach the last known player position.
- Return to Patrol: When the guard loses sight of the player or a specified delay is reached, it returns to patrolling.
- Noise Detection: The guard can detect loud noises made by the player, alerting them and initiating pursuit.

## Usage

1. Attach the `GuardAI` script to the guard character in your scene.
2. Set up waypoints as described above the guardAI will get the list of waypoints from the WaypointManager class.
4. Ensure the player character has the appropriate tag ("Player" tag) for detection.
5. Customize the various parameters in the `GuardAI` component to adjust the guard's behavior.

## Public Parameters

1. Waypoints: An array of Waypoint objects representing the patrol route for the guard. These waypoints define the path the guard follows during patrolling.
2. Rotation Speed: (float) The speed at which the guard rotates towards the target waypoint or player position.
3. Stopping Distance: (float) The minimum distance from the target waypoint at which the guard considers itself "arrived" and moves on to the next waypoint.
4. Move Speed: (float) The movement speed of the guard when patrolling between waypoints.
5. Return to Patrol Delay: (float) The delay in seconds before the guard returns to patrolling after losing sight of the player.
6. Target Offset: (float) The distance at which the guard stops near the player's last known position during pursuit.
7. Sound Detection Radius: (float) The radius around the guard within which it can detect loud noises made by the player.
8. Guard Layer Mask: (LayerMask) A layer mask to define the layers that the guard should consider when performing raycasts for player detection and noise detection. This is useful to exclude certain objects or layers from detection checks.

##  Add New Behaviors

- Simply can add new behavior adding a new state to the GuardState enum.
- you can code the new behaviour Functionality or can simply use already written functionality in the GuardAI class as every function has a simple one Job functionality.


# Test2 DOCUMENTATION
Brief break down of task.

## Task Break Down With Time
As I have a full time Job so I just completed this Simple AI on the weekend (Sunday).

1. Setting Up Environement. All environment player and guard are made of primitive shapes. (15 mins).
2. Waypoints System. Include Placing them (20 mins).
3. Setting Up Player Controller (1h).
4. Setting Cine Machine Virtual Camera (5 mins).
5. Moving AI on Way points smoothy (30 mins).
6. Baking NavMesh and setting up NaVmesh Obstacles (20 mins).
7. Detecting player (30 mins).
8. Back to nearest Waypoint (30 mins).
9. Setting Noise Manager (30 mins).
10. Detect Noise (10 mins).
11. Cleaning Code (10 mins).
12. Debug Logic and resolve bugs (1h).


## Futher Polish Feature
- The feature i would like to polish further is when the AI stop chasing it must move the nearest part between two waypoints.
- Should shift to Solid design Principle but due to time shortage just made it a simple state AI.

## Issues and Blockers
- Sometimes due to restricted area the AI can not reach the player and got stuck cos the AI posiiton was not <= target posiiton.
- If player Jumps behind the AI it didnt got detected resolved it by adding layer mask.

## Future Implementation
- Improved AI code using SOLID design principle.