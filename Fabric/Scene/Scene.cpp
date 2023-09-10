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

        // Component storage
        std::unordered_map<component_id, utl::sparse_set> _component_registry;
    }

    entity_id create_entity()
    {
        if (_free_count)
        {
            entity_id id = id::new_generation(_next);
            id::id_type index = id::index(id);
            
            _next = _registry[index];
            _registry[index] = id;
            
            index = id::index(_next);

            if (index != id::invalid_index)
            {
                id::id_type generation = id::generation(_registry[index]);
                _next = id::new_identifier(index, generation);
            }
            else
                _next = id::invalid_id;

            _free_count--;

            return id;
        }

        size_t index = _registry.size();
        u32 generation = 0;

        entity_id id = _registry.emplace_back(id::new_identifier(index, generation));

        return id;
    }

    void remove_entity(entity_id id)
    {
        assert(is_alive(id));

        id::id_type index = id::index(id);
        id::id_type generation = id::generation(id);
        id::id_type next_index = id::is_valid(_next) ? id::index(_next) : id::invalid_index;

        _registry[index] = id::new_identifier(next_index, generation);

        _next = id;
        _free_count++;
    }

    bool is_alive(entity_id id)
    {
        id::id_type index = id::index(id);
        id::id_type generation = id::generation(id);

        return id::generation(_registry[index]) == generation;
    }

    bool has_component(entity_id id, component_id component)
    {
        assert(_component_registry.contains(component));

        if (_component_registry.contains(component) && _component_registry[component].has(id))
            return true;

        return false;
    }

    void add_component(component& component)
    {
        if (!_component_registry.contains(component.id))
            _component_registry[component.id] = utl::sparse_set::create(component.size);

        _component_registry[component.id].emplace(component.owner, component.data);
    }

    void remove_component(entity_id id, component_id component)
    {
        assert(has_component(id, component));

        if (has_component(id, component))
        {
            _component_registry[component].remove(id);
        }
    }

    void* get_component(entity_id id, component_id component)
    {
        assert(has_component(id, component));

        if (has_component(id, component))
            return _component_registry[component][id];

        return nullptr;
    }
}

