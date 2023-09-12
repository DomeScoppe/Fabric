#pragma once

#include "Entity.h"

namespace fabric::ecs
{
	TYPED_ID(system_id);

	entity_id create_entity();
	void remove_entity(entity_id id);
	bool is_alive(entity_id id);

	system_id register_system(component_id owner, void(*function)());
	void add_dependency(system_id id, component_id dependency);
	void run_systems();
}