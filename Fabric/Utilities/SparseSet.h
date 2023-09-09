#pragma once

#include "Common/CommonHeaders.h"
#include "Common/Identifier.h"

namespace fabric::utl
{
	class sparse_set
	{
	public:
		sparse_set(size_t component_size, size_t size = 1024)
			: _component_size(component_size), _size(size)
		{
			resize(size);
		}

		void emplace(id::id_type id, void* data)
		{
			assert(id::is_valid(id));

			if (id::is_valid(id))
			{
				u32 index = id::index(id);

				if (_size > index)
				{
					// emplace logic
				}
				else
				{
					// resize if needed
				}
			}
		}

		bool has(id::id_type id)
		{
			assert(id::is_valid(id));

			if (id::is_valid(id))
			{
				u32 index = id::index(id);

				if (_size > index)
				{
					return _sparse[index] != id::invalid_id;
				}
			}

			return false;
		}

		size_t size() { return _size; }
		bool empty() { return _dense == nullptr; }

		id::id_type* dense() { return _dense; }
		void* component() { return _component; }

		void resize(u32 size)
		{
			// resize buffers
		}

		void* operator[](id::id_type id)
		{
			// access component of entity (id)
		}

		template<typename T>
		T& get(id::id_type id)
		{
			return (T)((*this)[id]);
		}

	private:
		u32 _component_size = 0;
		u32 _size = 0;
		u32* _sparse = nullptr;
		id::id_type* _dense = nullptr;
		void* _component = nullptr;
	};
}