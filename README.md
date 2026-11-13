Helicopter flight prototype
Unity 6000.2.7f2, HDRP, Input System. Primitives only.

Controls
Key	Action
W / S	pitch forward / back
A / D	roll left / right
Space / Left Ctrl	climb / descend
Q / E	yaw left / right
R	restart
The rotor needs a few seconds to spool up after start. Hold Space to lift off. To land, hold Ctrl until the skids touch; the collective drops to zero on the ground.

Flight model
Everything goes through one Rigidbody, no direct velocity or transform writes.

Lift / thrust. A single force along the rotor axis (transform.up): T = collective * maxThrust * rpm^2 * groundEffect. Horizontal movement only comes from tilting the disc, there is no separate "forward" force.
Gravity. Standard Rigidbody gravity.
Drag. Quadratic, computed per body axis with its own Cd*A, so the airframe is slippery nose-first and draggy sideways and vertically. Rigidbody linear damping is set to 0.
Torque. The main rotor puts a reaction torque on the fuselage proportional to thrust. The tail rotor cancels it and gives yaw control on top; its thrust is limited, so with a big collective change you can see the nose swing a bit.
Inertia. Mass and inertia tensor come from the Rigidbody. Rotor rpm and the collective lever both have their own lag.
Ground effect. Up to +15% thrust, fading out at one rotor diameter.
Pilot assist
Flying raw collective/cyclic on a keyboard is not much fun, so input goes through a small stability layer (FlightComputer):

WASD commands a tilt angle (up to 25 deg), held by a PD loop on pitch and roll.
Space/Ctrl command a vertical speed. With no input the target is 0 m/s, which is the hover. Collective is also corrected by 1/cos(tilt) so the helicopter does not sink when it leans.
Q/E command a yaw rate.
The assist only produces rotor commands. The forces themselves are applied in Helicopter and are limited by what the rotors can physically give.

Code layout
Scripts/
  Input/   PilotInput, IPilotInputSource, PlayerPilotInput
  Flight/  HelicopterConfig (ScriptableObject), FlightComputer, Aerodynamics, Helicopter
  View/    FollowCamera, RotorSpin, FlightHud
FlightComputer and Aerodynamics have no scene dependencies. Helicopter reads the Rigidbody state, asks the computer for a command and applies forces in FixedUpdate. Input is behind an interface, so a gamepad or a scripted pilot is a new component, not a change in the flight code. All tuning lives in HelicopterConfig.

Known simplifications
Tail rotor is applied as a pure yaw torque by default. The real side force (and the drift it causes in hover) can be switched on in the config.
No blade flapping, translational lift, vortex ring state or retreating blade stall.
Inertia tensor is assumed to be aligned with the body axes.
No wind.

What I would do next
Edit mode tests for FlightComputer and Aerodynamics.
Gamepad bindings and an assist on/off switch.
Translational lift and a wind model.
Shader rotations for spin
Add DI
