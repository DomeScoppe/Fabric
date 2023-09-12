#pragma once

#include <concepts>

namespace fabric
{
	template<typename T, typename U>
	concept IsNotSame = !std::is_same_v<T, U>;

	template<typename T, typename U>
	concept IsSame = std::is_same_v<T, U>;
}