#ifndef CURVEDHELPER
#define CURVEDHELPER

float _WorldRadius;
float4 _WorldCenter;

float3 slerp(float3 p0, float3 p1, float t)
{
	float cosHalfTheta = dot(p0, p1);

	float halfTheta = acos(cosHalfTheta);
	float sinHalfTheta = sqrt(1.0f - cosHalfTheta * cosHalfTheta);

	float ratioA = sin((1.0f - t) * halfTheta) / sinHalfTheta;
	float ratioB = sin(t * halfTheta) / sinHalfTheta;

	return p0 * ratioA + p1 * ratioB;
}

float4 curve(float4 vertex, float radius, float3 center)
{
#ifdef NO_CURVE
	return vertex;
#else
	float3 toVertex = vertex.xyz - center;
	float d = length(toVertex.xz);

	// angle on a sphere = XZ distance / circumfrence = (d / 2 pi r) * 2 pi radians
	float angle = d / radius;
	float s, c;
	sincos(angle, s, c);

	// normalized vector rotated by angle from up towards XZ
	vertex.xz = (s / d) * toVertex.xz;
	vertex.y = c;

	// final position 
	vertex.xyz = (vertex.xyz * toVertex.y) + center;
	return vertex;
#endif
}

float3 inverseCurve(float3 worldPos, float radius, float3 center)
{
#ifdef NO_CURVE
	return worldPos;
#else
	float3 spherePos = worldPos - center.xyz;

	worldPos.y = length(spherePos);
	float3 norm = spherePos / worldPos.y;

	float angle = acos(norm.y);

	float d = angle * radius;
	worldPos.xz = normalize(spherePos.xz) * d;

	worldPos += center;

	return worldPos;
#endif
}

float4 curve(float4 vertex, float4x4 Object2World, float4x4 World2Object, float radius, float3 center)
{
#ifdef NO_CURVE
	return vertex;
#else
	vertex = mul(Object2World, vertex);
	vertex = curve(vertex, radius, center);
	return mul(World2Object, vertex);
#endif
}

float4 curveVertex(float4 vertex, float4x4 Object2World, float4x4 World2Object)
{
	return curve(vertex, Object2World, World2Object, _WorldRadius, _WorldCenter.xyz);
}

float4 curveVertexFixed(float4 vertex, float4x4 Object2World, float4x4 World2Object, float radius, float3 center)
{
	return curve(vertex, Object2World, World2Object, radius, center);
}

float4 curveVertexFixed(float4 vertex, float radius, float3 center)
{
	return curve(vertex, radius, center);
}

float3 inverseCurve(float3 vertex)
{
	return inverseCurve(vertex, _WorldRadius, _WorldCenter.xyz);
}

#endif
