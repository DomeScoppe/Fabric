#pragma once

#include "../GameEntity/Common.h"
#include "../GameEntity/Entity.h"

namespace fabric::entity
{
	class entity
	{
	public:
		constexpr explicit entity(entity_id id) : _id{ id } {}
		constexpr entity() : _id{ id::invalid_id } {}

		constexpr entity_id get_id() const { return _id; }

		template<typename T>
		constexpr bool has_component()
		{
			const component_id comp = get_component_id<T>();
			return detail::has_component(_id, comp);
		}

		template<typename T>
		constexpr T& get_component() const
		{
			const component_id comp = get_component_id<T>();
			return *(static_cast<T*>(detail::get_component(_id, comp)));
		}

		template<typename T>
		constexpr void add_component(const T& data) const
		{
			const component_id comp = get_component_id<T>();
			detail::add_component(_id, comp, &data, sizeof(T));
		}

		template<typename T>
		constexpr void remove_component() const
		{
			const component_id comp = get_component_id<T>();
			detail::remove_component(_id, comp);
		}

	private:
		entity_id _id;
	};
}