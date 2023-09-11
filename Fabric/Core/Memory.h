#pragma once

namespace fabric::memory
{
	namespace pool_allocator
	{
		void* allocate(size_t size);
		void free(void* block);
	}
}