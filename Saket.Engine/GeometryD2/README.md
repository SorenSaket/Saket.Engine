<h1>Geometry 2D</h1> 
Speed > Extendability

<h2>Rects and Bounding Boxes</h2> 
A rect is a "rotatable box" consists of position, size and rotation.

A Boundingbox or AABB, Axis Aligned Bounding Box uses Min and Max to determine the poistion and size.

You can convert a rect to a bounding box by using `rect.GetBounds()`;

A the components of a bounding box max is always bigger than min.

<svg width="200" height="100">
	<rect x="0" y="0" width="200" height="100" stroke="DarkOrchid" stroke-width="6" fill="DarkSlateGrey" />
	<circle cx="4" cy="96" r="4"  stroke-width="4" fill="green" />
	<circle cx="196" cy="4" r="4"  stroke-width="4" fill="red" />
	<text fill="green" font-size="16" font-family="Verdana" x="8" y="90">Min</text>
	<text fill="red" font-size="16" font-family="Verdana" x="156" y="18">Max</text>
	<text fill="white" font-size="16" font-family="Verdana" x="14" y="54">Bounding Box (AABB)</text>
</svg>


<h2>Terminology</h2> 
<div style="background:DarkSlateGray; width:50%; padding:16px;">
<ul>
	<li>Shape is derived from a list of contours.</li>
	<li>Contour is derived from a list of curves.</li>
	<li>Curves are dervied from a list of points.</li>
</ul>
</div>

<h4>Shape</h4> 
Can be used to describe a mathematical shape. For letters, Graphics, etc
Consists of multiple contours.
	
<h4>Contours</h4> 
"an outline, especially one representing or bounding the shape or form of something."

<h4>Spline</h4> 
"a continuous curve constructed so as to pass through a given set of points and have a certain number of continuous derivatives."
Simply a list of vector2's that can be interpreted in different ways. A collection of curves
<h4>Curve</h4> 
"an abstract term used to describe the path of a continuously moving point"


<h2>Public functions</h2> 

#### Evaluate
Returns the coordiates a percentage along the spline. t = 0..1

#### Bounds
returns an axis aligned bounding box around the spline

#### Winding
winding direction. Clockwise = 1, counterclockwise = -1

https://github.com/FreyaHolmer/Mathfs/tree/master/Runtime/Splines



