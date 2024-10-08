Shape
	Can be used to describe a mathematical shape. For letters, Graphics, etc.
	Consists of multiple contours.
	
Contours
	"an outline, especially one representing or bounding the shape or form of something."

Spline
	"a continuous curve constructed so as to pass through a given set of points and have a certain number of continuous derivatives."
	Simply a list of vector2's that can be interpreted in different ways. A collection of curves
Curve
	"an abstract term used to describe the path of a continuously moving point"


Shape is derived from a list of contours
a contour is derived from a list of curves.
Curves are dervied from a list of points.




All Splines are simply defined as a set of points. 
The way these points can be interpreted as a continuous curve can be different.

Speed > Extendability

Public functions:

#### Evaluate
Returns the coordiates a percentage along the spline. t = 0..1

#### Bounds
returns an axis aligned bounding box around the spline

#### Winding
winding direction. Clockwise = 1, counterclockwise = -1



https://github.com/FreyaHolmer/Mathfs/tree/master/Runtime/Splines



