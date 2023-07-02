#pragma once

#include "CommonHeaders.h"

namespace fabric::id
{
	using id_type = u64;

	namespace detail
	{
		constexpr u32 generation_bits = 16;
		constexpr u32 index_bits = 32;

		constexpr id_type index_mask = (id_type(1) << index_bits) - 1;
		constexpr id_type generation_mask = (id_type(1) << generation_bits) - 1;
	}

	constexpr id_type invalid_id = id_type(-1);
	constexpr u32 min_deleted_elements = 1024;

	using generation_type = std::conditional_t<detail::generation_bits <= 16, std::conditional_t<detail::generation_bits <= 8, u8, u16>, u32>;
	static_assert(sizeof(generation_type) * 8 >= detail::generation_bits);
	static_assert(sizeof(id_type) * 8 >= detail::generation_bits + detail::index_bits);

	constexpr bool is_valid(id_type id)
	{
		return id != invalid_id;
	}

	constexpr id_type index(id_type id)
	{
		id_type index = id & detail::index_mask;
		assert(index != invalid_id);
		return index;
	}

	constexpr id_type generation(id_type id)
	{
		return (id >> detail::index_bits) & detail::generation_mask;
	}

	constexpr id_type new_generation(id_type id)
	{
		const id_type generation = id::generation(id) + 1;
		assert(generation < ((u64)1 << detail::generation_bits) - 1);
		return index(id) | (generation << detail::index_bits);
	}

	constexpr id_type index_pair(id_type value1)
	{
		id_type index = value1;
		assert(index != id::invalid_id);

		return index;
	}

	constexpr id_type index_pair(id_type value1, id_type value2)
	{
		id_type index = value1;
		index |= value2 << detail::index_bits;
		assert(index != id::invalid_id);

		return index;
	}

	constexpr id_type pair_value1(id_type id)
	{
		id_type index = id & detail::index_mask;
		assert(index != id::invalid_id);

		return index;
	}

	constexpr id_type pair_value2(id_type id)
	{
		id_type index = (id >> detail::index_bits) & detail::index_mask;
		assert(index != id::invalid_id);

		return index;
	}

#if _DEBUG
	namespace internal
	{
		struct id_base
		{
		public:
			constexpr explicit id_base(id_type id) : _id{ id } {}
			constexpr operator id_type() const { return _id; }
			std::size_t hash() const { return (std::size_t)_id; }

		private:
			id_type _id;
		};

#define TYPED_ID(name)													\
		struct name final : id::internal::id_base						\
		{																\
			constexpr explicit name(id::id_type id) : id_base{ id } {}	\
			constexpr name() : id_base{ 0 } {}							\
		};
	}
	
#else
#define TYPED_ID(name) using name = id::id_type;
#endif
}