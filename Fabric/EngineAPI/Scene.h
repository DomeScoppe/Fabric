#pragma once

#include "../GameEntity/Common.h"
#include "../GameEntity/Entity.h"
#include "EngineAPI/GameEntity.h"

namespace fabric::scene
{
	entity::entity create_entity();
	void remove_entity(entity::entity entity);
	bool is_alive(entity::entity_id id);

	template<typename T>
	constexpr std::span<T> get_all()
	{
		const entity::component_id comp = entity::get_component_id<T>();
		u32 count = 0;
		T* data = static_cast<T*>(entity::get_all_components(comp, count));

		return std::span<T>({ data, count });
	}
}