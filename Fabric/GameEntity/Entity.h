#pragma once

#include "Common.h"

namespace fabric::entity
{
	TYPED_ID(entity_id);
	TYPED_ID(component_id);
	extern u32 _componentCounter;

	template<typename T>
	const component_id get_component_id()
	{
		static component_id id = component_id(_componentCounter++);
		return id;
	}

	void register_entity(entity_id id);
	void unregister_entity(entity_id id);

	void* get_all_components(component_id component, u32& count);

	namespace detail
	{
		bool has_component(entity_id entity, component_id component);
		void* get_component(entity_id entity, component_id component);
		void add_component(entity_id entity, component_id component, const void* data, u64 size);
		void remove_component(entity_id entity, component_id component);
	}
}

namespace std
{
	template<>
	struct hash<fabric::entity::entity_id>
	{
		size_t operator()(fabric::entity::entity_id const& id) const noexcept { return id.hash(); }
	};
	template<>
	struct hash<fabric::entity::component_id>
	{
		size_t operator()(fabric::entity::component_id const& id) const noexcept { return id.hash(); }
	};
}