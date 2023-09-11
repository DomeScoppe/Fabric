#include "Memory.h"

#include <memory>

namespace fabric::memory
{
    namespace
    {

    }

    void* pool_allocator::allocate(size_t size)
    {
        return malloc(size);
    }

    void pool_allocator::free(void* block)
    {
        ::free(block);
        block = nullptr;
    }
}

