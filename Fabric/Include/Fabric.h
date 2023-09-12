#pragma once

#include "Engine.h"

namespace fabric
{
	namespace entity
	{
		class entity
		{
		public:
			explicit entity() : _id{ id::invalid_id } {}
			explicit entity(ecs::entity_id id) : _id(id) {}

			ecs::entity_id get_id() { return _id; }

			template<typename Component>
			constexpr bool has_component()
			{
				return ecs::has_component(_id, ecs::get_component_id<Component>());
			}

			template<typename Component>
			constexpr void add_component()
			{
				ecs::component component
				{
					.owner = _id,
					.id = ecs::get_component_id<Component>(),
					.size = sizeof(Component)
				};

				ecs::add_component(component);
			}

			template<typename Component>
			constexpr void add_component(Component& component)
			{
				ecs::component comp
				{
					.owner = _id,
					.id = ecs::get_component_id<Component>(),
					.data = &component,
					.size = sizeof(Component)
				};

				ecs::add_component(comp);
			}

			template<typename Component>
			constexpr const Component& get_component()
			{
				Component* component = (Component*) ecs::get_component(_id, ecs::get_component_id<Component>());

				if (component)
					return *component;

				return Component();
			}

			template<typename Component>
			constexpr void remove_component()
			{
				ecs::remove_component(_id, ecs::get_component_id<Component>());
			}

		private:
			ecs::entity_id _id;
		};
	}

	namespace scene
	{
		entity::entity create_entity()
		{
			return entity::entity(ecs::create_entity());
		}

		void remove_entity(entity::entity e)
		{
			ecs::remove_entity(e.get_id());
		}
	}
}
