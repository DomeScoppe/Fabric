#pragma once

#include "Test.h"

#include "GameEntity/Entity.h"
#include "GameEntity/Transform.h"

#include <iostream>
#include <ctime>

using namespace fabric;

class engine_test : public test
{
public:
	virtual bool initialize() override 
	{
		srand((u32)time(nullptr));

		return true;
	}

	virtual void run() override 
	{ 
		do
		{
			for (u32 i{ 0 }; i < 10000; ++i)
			{
				create_random();
				remove_random();
				_num_entities = (u32)_entities.size();
			}

			print_results();

		} while (getchar() != 'q');
	}

	virtual void shutdown() override 
	{  

	}

private:
	void create_random()
	{
		u32 count = rand() % 20;

		if (_entities.empty()) count = 1000;
		transform::init_info transform_info{};
		entity::entity_info entity_info
		{
			.transform = &transform_info,
		};

		while (count > 0)
		{
			++_added;
			entity::entity entity{ entity::create(entity_info) };

			assert(entity.is_valid() && id::is_valid(entity.get_id()));

			_entities.push_back(entity);

			assert(entity::is_alive(entity.get_id()));

			--count;
		}
	}

	void remove_random()
	{
		u32 count = rand() % 20;
		if (_entities.size() < 1000) return;

		while (count > 0)
		{
			const u32 index{ (u32)rand() % (u32)_entities.size() };
			const entity::entity entity{ _entities[index] };

			assert(entity.is_valid() && id::is_valid(entity.get_id()));

			if (entity.is_valid())
			{
				entity::remove(entity.get_id());
				_entities.erase(_entities.begin() + index);

				assert(!entity::is_alive(entity.get_id()));

				++_removed;
			}

			--count;
		}
	}

	void print_results()
	{
		std::cout << "Entities created: " << _added << std::endl;
		std::cout << "Entities removed: " << _removed << std::endl;
	}

private:
	utl::vector<entity::entity> _entities;

	u32 _added{ 0 };
	u32 _removed{ 0 };
	u32 _num_entities{ 0 };
};