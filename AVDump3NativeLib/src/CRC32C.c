#include "AVD3NativeLibApi.h"

void* CRC32CCreate(uint32_t* hashLength, uint32_t* blockSize) {
	*blockSize = 8;
	*hashLength = 32;

	uint8_t *b = (uint8_t*)malloc(4);
	CRC32CInit(b);
	return b;
}

void CRC32CInit(void * handle) {
	memset((uint8_t*)handle, 0xFF, 4);
}

void CRC32CTransform(void* handle, uint8_t *b, int32_t length) {
	uint64_t state64 = *(uint32_t*)handle;

	uint64_t* words = (uint64_t*)b;
	uint64_t* wordEnd = words + (length >> 3);
	while (words != wordEnd) {
		state64 = _mm_crc32_u64(state64, *(words++));
	}
}

void CRC32CFinal(void* handle, uint8_t* b, int32_t length, uint8_t* hash) {
	uint32_t state32 = *(uint32_t*)handle;
	uint8_t* bEnd = b + (length & 7);
	while (b != bEnd) {
		state32 = _mm_crc32_u8(state32, *(b++));
	}
	*(uint32_t*)handle = state32;


	*((int32_t*)hash) = *(int32_t*)handle ^ 0xFFFFFFFF;
}
