/*
 * memory.h
 *
 *  Created on: 2010/06/18
 *      Author: brNX
 */

#ifndef MEMORY_H_
#define MEMORY_H_

#include "types.h"
#include "cartridge.h"


#ifdef __cplusplus
extern "C"
{
#endif

    typedef struct _Memory
    {
        uint8 * rombanks;
        uint8 * rambanks;

        uint8 wram[8192];
        uint8 hram[127];
        uint8 vram[8192];
        uint8 OAM[160];
        uint8 IO[128];
        uint8 rie;
        uint8 rif;

        int timer1;
        int timer2;

        Cartridge * cart;
    }Memory;

    INLINE uint8 readMem(uint16 address,Memory * mem);
    INLINE void writeMem(uint16 address, uint8 value,Memory * mem);
    INLINE void lcdDMA(uint8 value,Memory * mem);
    INLINE void writeToIOZone(uint16 address, uint8 value,Memory * mem);
    INLINE void romBankSwitch(uint16 address, uint8 value,Memory * mem);
    INLINE void ramBankSwitch(uint16 address, uint8 value,Memory * mem);
    INLINE uint8 getJoypadState(Memory * mem);
    void printMEMStatus(Memory * mem);
    void initMemory(Memory* mem,Cartridge * cart);
    void destroyMemory(Memory * mem);



#ifdef __cplusplus
}
#endif

#endif /* MEMORY_H_ */
