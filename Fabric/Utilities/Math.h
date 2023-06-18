#pragma once

#include "../Common/CommonHeaders.h"

namespace fabric::math
{
	constexpr f32 pi = 3.1415926535897932384626433832795f;
	constexpr f32 epsilon = 1e-5f;

	using float2 = DirectX::XMFLOAT2A;
	using float3 = DirectX::XMFLOAT3A;
	using float4 = DirectX::XMFLOAT4A;	
	using uint2 = DirectX::XMUINT2;
	using uint3 = DirectX::XMUINT3;
	using uint4 = DirectX::XMUINT4;	
	using int2 = DirectX::XMINT2;
	using int3 = DirectX::XMINT3;
	using int4 = DirectX::XMINT4;
	using float3x3 = DirectX::XMFLOAT3X3;
	using float4x4 = DirectX::XMFLOAT4X4A;

	using vec2 = DirectX::XMFLOAT2;
	using vec3 = DirectX::XMFLOAT3;
	using vec4 = DirectX::XMFLOAT4;
	using uvec2 = uint2;
	using uvec3 = uint3;
	using uvec4 = uint4;	
	using ivec2 = int2;
	using ivec3 = int3;
	using ivec4 = int4;
	using mat3 = float3x3;
	using mat4 = DirectX::XMFLOAT4X4;
	using quat = vec4;
}