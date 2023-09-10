#pragma once

#include "Common/CommonHeaders.h"
#include "Common/Identifier.h"

namespace fabric::utl
{
	class sparse_set
	{
	public:
		sparse_set() = default;

		sparse_set(size_t component_size, size_t size = 1024)
			: _component_size(component_size)
		{
			resize(size);
		}

		sparse_set(const sparse_set& other)
		{
			_component_size = other._component_size;
			
			resize(other._size);

			memcpy_s(_sparse, _size * sizeof(id::id_type), other._sparse, _size * sizeof(id::id_type));
			memcpy_s(_dense, _size * sizeof(id::id_type), other._dense, _size * sizeof(id::id_type));
			memcpy_s(_component, _size * _component_size, other._component, _size * _component_size);
		}

		sparse_set& operator=(const sparse_set& other)
		{
			_component_size = other._component_size;

			resize(other._size);

			memcpy_s(_sparse, _size * sizeof(id::id_type), other._sparse, _size * sizeof(id::id_type));
			memcpy_s(_dense, _size * sizeof(id::id_type), other._dense, _size * sizeof(id::id_type));
			memcpy_s(_component, _size * _component_size, other._component, _size * _component_size);

			return *this;
		}

		~sparse_set()
		{
			_component_size = 0;
			_size = 0;

			// TODO: Memory should be returned to a memory pool, not to the OS
			free(_sparse);
			free(_dense);
			free(_component);

			_sparse = nullptr;
			_dense = nullptr;
			_component = nullptr;
		}

		static sparse_set create(size_t component_size, size_t size = 1024)
		{
			return sparse_set(component_size, size);
		}

		void emplace(id::id_type id, void* data)
		{
			assert(id::is_valid(id));

			if (id::is_valid(id))
			{
				id::id_type index = id::index(id);

				if (_size > index)
				{
					size_t next_component = (_next_component - (char*)_component) / _component_size;

					if (id::is_valid(_sparse[index]))
					{
						next_component = _sparse[index];
					}

					_sparse[index] = next_component;
					_dense[next_component] = id;

					if(data)
						memcpy_s((char*)_component + next_component * _component_size, _component_size, data, _component_size);

					_next_component = (char*)_component + (next_component + 1) * _component_size;
				}
				else
				{
					resize(2 * _size);
					emplace(id, data);
				}
			}
		}

		bool has(id::id_type id)
		{
			assert(id::is_valid(id));

			if (id::is_valid(id))
			{
				id::id_type index = id::index(id);

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

		void resize(size_t size)
		{
			// TODO: Memory should be allocated from a memory pool, not from the OS

			id::id_type* sparse = (id::id_type*) malloc(size * sizeof(id::id_type));
			id::id_type* dense = (id::id_type*) malloc(size * sizeof(id::id_type));
			void* component = malloc(size * _component_size);

			if(sparse)
				memset(sparse, -1, size * sizeof(id::id_type));

			if (dense)
				memset(dense, 0, size * sizeof(id::id_type));

			if (component)
				memset(component, 0, size * _component_size);

			if (_sparse)
			{
				assert(_dense && _component);

				memcpy_s(sparse, size * sizeof(id::id_type), _sparse, _size);
				memcpy_s(dense, size * sizeof(id::id_type), _dense, _size);
				memcpy_s(component, size * _component_size, _sparse, _size);

				// TODO: Memory should be returned to a memory pool, not to the OS
				free(_sparse);
				free(_dense);
				free(_component);
			}

			_next_component = (char*)component + (_size * _component_size);
			_sparse = sparse;
			_dense = dense;
			_component = component;
			_size = size;
		}

		void* operator[](id::id_type id)
		{
			id::id_type index = _sparse[id::index(id)];

			if (id::is_valid(index))
				return (char*)(char*)_component + index * _component_size;

			return nullptr;
		}

		void remove(id::id_type id)
		{
			assert(id::is_valid(id));

			if (id::is_valid(id))
			{
				id::id_type index = _sparse[id::index(id)];
				size_t last = ((_next_component - (char*)_component) / _component_size);
				
				if (id::is_valid(index) && last > 0)
				{
					last = last - 1;
					_next_component -= _component_size;

					_dense[index] = _dense[last];
					_dense[last] = id;
				
					void* temp = malloc(_component_size);

					memcpy_s(temp, _component_size, (char*)_component + index * _component_size, _component_size);
					memcpy_s((char*)_component + index * _component_size, _component_size, (char*)_component + last * _component_size, _component_size);
					memcpy_s((char*)_component + last * _component_size, _component_size, temp, _component_size);

					free(temp);

					_sparse[id::index(_dense[last])] = index;
					_sparse[id::index(id)] = last;
				}
			}
		}

	private:
		size_t _component_size = 0;
		size_t _size = 0;
		char* _next_component = nullptr;
		id::id_type* _sparse = nullptr;
		id::id_type* _dense = nullptr;
		void* _component = nullptr;
	};
}