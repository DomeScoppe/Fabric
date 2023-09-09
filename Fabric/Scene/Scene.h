#pragma once

#include "Entity.h"

namespace fabric::ecs
{
	entity_id create_entity();
	void remove_entity(entity_id id);
	bool is_alive(entity_id id);
}