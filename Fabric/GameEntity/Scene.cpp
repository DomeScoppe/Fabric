#include "EngineAPI/Scene.h"

namespace fabric::scene
{
	namespace
	{
		utl::vector<id::generation_type> generations;
		utl::deque<entity::entity_id> free_ids;
	}

	entity::entity create_entity()
	{
		entity::entity_id id;

		// reuse old entity_id with increased generation or create new entity_id
		if (free_ids.size() > 0)
		{
			id = free_ids.front();
			assert(!is_alive(id));
			free_ids.pop_front();
			id = entity::entity_id{ id::new_generation(id) };
		}
		else
		{
			id = entity::entity_id{ id::id_type(generations.size()) };
			generations.push_back(id::generation_type{ 0 });
		}

		entity::register_entity(id);

		return entity::entity{ id };
	}

	void remove_entity(entity::entity entity)
	{
		entity::entity_id id = entity.get_id();
		assert(is_alive(id));
		if (is_alive(id))
		{
			entity::unregister_entity(id);
			free_ids.push_back(id);
			++generations[id::index(id)];
		}
	}

	bool is_alive(entity::entity_id id)
	{
		assert(id::is_valid(id));
		const id::id_type index = id::index(id);
		assert(index < generations.size());

		return generations[index] == id::generation(id);
	}
}