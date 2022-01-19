# Gravity and cloth simulation

[Demonstration](https://youtu.be/OgrpkbtYH7w)

## Force simulated :
- Gravity
- Springs with directional dampening
- Bouncing
- Resitance

## Simulation :

### Initialisation phase :
1) Raylib
2) 3D Model
3) Univers
4) GPU

### Main loop:

#### 1) Simulation
1) Gravity for every particule
2) Spring Forces for every springs
3) Apply spring forces on the particule
4) Update velocity for each particule
5) Update position for each particule
6) check collision for every particule

We download and send the data to the GPU between every action

#### 2) Display

1) Draw the spings
2) Draw the particule

to draw the particule we take the array of particule and the array of color
They are the same length and particule[i] -> color[i]

## CONTROLE
```
[SPACE]                     -> start the simulation
[LEFT]                      -> Display debug
[UP] or [DOWN]              -> Change targeted particule
[KEY PAD +] or [KEY PAD -]  -> Slow or accelerate the simulation (When accelerated -> can be unstable)
```

## OPENCL
The Object GPU is an interface to communicate with the gpu
All the kernel are in the opencl folder

I had to reimplement a bunch of basic vector function because I am using Vector3 witch is from C# and differ from float3 in OpenCl

### Kernel :
#### [particule_gravity]     :   Compute the gravity for each particule
```
0 : Univer struct
1 : Input buffer (array of particule)
2 : Output buffer (array of particuke)
```
exectuted once for each particule

#### [particul_spring]       :   Compute the force for each springs
```
0 : Input buffer (array of particule)
1 : springs -> array containing all the springs
2 : sp -> array of springs_force
```
exectuted once for every springs

#### [spring_applier]        :   Apply the force to each particule of the cloth
```
0 : Input buffer (array of particule)
1 : Output buffer (array of particule)
2 : sp -> array of springs_force
3 : cloth -> array of one element -> the cloth settings
```

Since all the cloth particule are in the same buffer as all the other particule
We need to know where they are in the array
- offset : tells where the index start
- count  : tells how many particule they are in the array
```
ex : [p,p,p,p,p,c,c,c,c,c,c,p,p];
    -> offset 5
    -> count  6
```
We know the cloth is from index 5 to index 10

Executed once for every particule in the cloth

#### [particule_velocity]    :   Apply the acceleration to the velocity
#### [particule_position]    :   Apply the velocity to the position
```
0 : Univers (array of one element) contain the univer parameter G and dt
1 : Input (array of all the particules)
2 : Ouput (array of all the particules)
```
They both are exectuted once per particule

#### [particul_collision]    :   Compute collision and adjust velocity and position
```
0 : Univers (array of one element) contain the univer parameter G and dt
1 : Input (array of all the particuls)
2 : Ouput (array of all the particuls)
```
Executed once for every particules

## Improvement
- don't download the buffer and keep every thing on the GPU (big FPS increase)
- Change data structure for an 3D map to improve collision check