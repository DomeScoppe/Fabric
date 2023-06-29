#include "Entity.h"

namespace fabric::entity
{
	u64 _componentCounter = 0;
	namespace
	{
		using component_map = std::unordered_map<component_id, u32>;
		using entity_set = std::unordered_set<entity_id>;
		using component_set = std::unordered_set<component_id>;

		struct component_metadata
		{
			void* data;
			u64 component_size;
			u32 count;
		};

		struct entity_metadata
		{
			component_set components;
			component_map component_index;
		};

		std::unordered_map<entity_id, entity_metadata> entity_registry;
		std::unordered_map<component_id, entity_set> component_mapping;
		std::unordered_map<component_id, component_metadata> component_storage;
	}

	void register_entity(entity_id id)
	{
		assert(!entity_registry.contains(id));

		if (!entity_registry.contains(id))
		{
			entity_metadata metadata;

			entity_registry[id] = metadata;
		}
	}

	void unregister_entity(entity_id id)
	{
		entity_metadata& metadata = entity_registry[id];
		auto iter = metadata.components.begin();

		for (u32 i = 0; i < metadata.components.size() && iter != metadata.components.end(); i++)
		{
			detail::remove_component(id, *iter);
			iter = metadata.components.begin();
		}

		entity_registry.erase(id);
	}

	void* get_all_components(component_id component, u32& count)
	{
		assert(component_storage.contains(component));
		count = 0;

		if (component_storage.contains(component))
		{
			count = component_storage[component].count;

			// TODO: request tight block of data from allocator
			return component_storage[component].data;
		}

		return nullptr;
	}

	namespace detail
	{
		bool has_component(entity_id entity, component_id component)
		{
			return component_storage.contains(component) && entity_registry[entity].components.contains(component);
		}

		void* get_component(entity_id entity, component_id component)
		{
			if (has_component(entity, component))
			{
				u32 index = entity_registry[entity].component_index[component];
				u64 size = component_storage[component].component_size;
				char* data = (char*)component_storage[component].data + size * index;

				return (void*)data;
			}

			return nullptr;
		}

		void add_component(entity_id entity, component_id component, const void* data, u64 size)
		{
			// if entity has this component, overwrite it
			if (has_component(entity, component))
			{
				u32 index = entity_registry[entity].component_index[component];
				void* component_data = component_storage[component].data;
				memcpy((char*)component_data + size * index, data, size);

				return;
			}

			u32 count = 0;

			if (component_storage.contains(component))
			{
				// if component is already on storage, add a new block of data
				// TODO: This should be done with a pool allocator

				count = component_storage[component].count++;
				void* data_block = malloc(size * (count + 1));
				void* stored_data = component_storage[component].data;
				memcpy(data_block, stored_data, size * count);
				memcpy((char*)data_block + size * count, data, size);
				component_storage[component].data = data_block;
				free(stored_data);
			}
			else
			{
				// if it's a new component type, create a slot for it
				// TODO: This should be done with a pool allocator

				component_metadata component_slot;
				component_slot.count = 1;
				component_slot.component_size = size;
				component_slot.data = malloc(size);
				memcpy(component_slot.data, data, size);

				component_storage[component] = component_slot;
			}

			// assign component to entity
			entity_registry[entity].components.insert(component);
			entity_registry[entity].component_index[component] = count;
			component_mapping[component].insert(entity);
		}

		void remove_component(entity_id entity, component_id component)
		{
			assert(component_storage.contains(component));

			if (has_component(entity, component))
			{
				u32 index = entity_registry[entity].component_index[component];
				u64 size = component_storage[component].component_size;
				u32 count = --component_storage[component].count;
				void* components = component_storage[component].data;

				// create new data block without removed slot
				// TODO: This should be done with a pool allocator

				void* data_block = malloc(size * count);
				memcpy(data_block, components, size * index);

				if (index < count)
				{
					memcpy(data_block, (char*)components + size * (index + 1), size * (count - index));
				}

				component_storage[component].data = data_block;

				free(components);

				entity_id to_remove;

				for (entity_id id : component_mapping[component] )
				{
					if (entity_registry[id].component_index[component] < index) continue;
					if (entity_registry[id].component_index[component] == index)
					{
						to_remove = id;
						continue;
					}

					--entity_registry[id].component_index[component];
				}

				component_mapping[component].erase(to_remove);

				entity_registry[entity].components.erase(component);
				entity_registry[entity].component_index.erase(component);
			}
		}

	}
}