#pragma once

#include "Common.h"

namespace fabric::ecs
{
	TYPED_ID(entity_id);

	struct component
	{
		entity_id owner;
		u64 type;
		void* data = nullptr;
		u64 size = 0;
	};

	bool has_component(entity_id id, u64 type);
	void add_component(component* component);
}

namespace std
{
	template<>
	struct hash<fabric::ecs::entity_id>
	{
		size_t operator()(fabric::ecs::entity_id const& id) const noexcept { return u64(id); }
	};
}