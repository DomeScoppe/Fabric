#pragma once

#include "CommonHeaders.h"

namespace fabric::id
{
	using id_type = u64;

	namespace detail
	{
		constexpr id_type index_bits = 32;
		constexpr id_type generation_bits = 32;

		static_assert(sizeof(id_type) * 8 == index_bits + generation_bits);

		constexpr id_type index_mask = (id_type(1) << index_bits) - 1;
		constexpr id_type generation_mask = (id_type(1) << generation_bits) - 1;
	}

	constexpr id_type invalid_id = id_type(-1);
	constexpr id_type invalid_index = id::detail::index_mask;
	constexpr id_type invalid_generation = id::detail::generation_mask;

	constexpr id_type new_identifier(id_type index, id_type generation)
	{
		return index | (generation << detail::index_bits);
	}

	constexpr bool is_valid(id_type id)
	{
		return id != invalid_id;
	}

	constexpr id_type index(id_type id)
	{
		return id & detail::index_mask;
	}

	constexpr id_type generation(id_type id)
	{
		return (id >> detail::index_bits) & detail::generation_mask;
	}

	constexpr id_type new_generation(id_type id)
	{
		id_type new_generation = generation(id) + 1;
		return index(id) | (new_generation << detail::index_bits);
	}

#define TYPED_ID(name) using name = id::id_type;
}