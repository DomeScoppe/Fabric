#include "Scene.h"
#include "Utilities/SparseSet.h"

namespace fabric::ecs
{
    namespace
    {
        // Entity storage
        utl::vector<entity_id> _registry;
        entity_id _next = id::invalid_id;
        u32 _free_count = 0;

        std::unordered_map<u64, utl::sparse_set> component_storage;
    }

    entity_id create_entity()
    {
        if (_free_count)
        {
            entity_id id = id::new_generation(_next);
            u32 index = id::index(id);
            
            _next = _registry[index];
            _registry[index] = id;
            
            index = id::index(_next);

            if (index != id::invalid_index)
            {
                u32 generation = id::generation(_registry[index]);
                _next = id::new_identifier(index, generation);
            }
            else
                _next = id::invalid_id;

            _free_count--;

            return id;
        }

        u32 index = _registry.size();
        u32 generation = 0;

        entity_id id = _registry.emplace_back(id::new_identifier(index, generation));

        return id;
    }

    void remove_entity(entity_id id)
    {
        assert(is_alive(id));

        u32 index = id::index(id);
        u32 generation = id::generation(id);
        u32 next_index = id::is_valid(_next) ? id::index(_next) : id::invalid_index;

        _registry[index] = id::new_identifier(next_index, generation);

        _next = id;
        _free_count++;
    }

    bool is_alive(entity_id id)
    {
        u32 index = id::index(id);
        u32 generation = id::generation(id);

        return id::generation(_registry[index]) == generation;
    }

}

