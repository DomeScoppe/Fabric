#pragma once

#include "Common.h"

namespace fabric::ecs
{
	extern u32 _componentCounter;

	TYPED_ID(entity_id);
	TYPED_ID(component_id);

	struct component
	{
		entity_id owner;
		component_id id;
		void* data = nullptr;
		u64 size = 0;
	};

	template<typename Component>
	const component_id get_component_id()
	{
		static component_id id = component_id(_componentCounter++);
		return id;
	}

	bool has_component(entity_id id, component_id component);
	void add_component(component& component);
	void remove_component(entity_id id, component_id component);
	void* get_component(entity_id id, component_id component);
}

namespace std
{
	template<>
	struct hash<fabric::ecs::entity_id>
	{
		size_t operator()(fabric::ecs::entity_id const& id) const noexcept { return u64(id); }
	};
}