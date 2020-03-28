import User from '@/models/user';
import Character from '@/models/character';
import Item from '@/models/item';
import { UpdateCharacterItemsRequest } from '@/models/update-character-items-request';
import { UpdateCharacterRequest } from '@/models/update-character-request';
import { get, post, put } from './crpg-client';

export function getUser(): Promise<User> {
  return get('/users/self');
}

export function getOwnedItems(): Promise<Item[]> {
  return get('/users/self/items');
}

export function updateCharacter(characterId: number, req: UpdateCharacterRequest) {
  return put(`/users/self/characters/${characterId}`, req);
}

export function updateItems(characterId: number, req: UpdateCharacterItemsRequest): Promise<Character> {
  return put(`/users/self/characters/${characterId}/items`, req);
}

export function buyItem(itemId: number) {
  return post('/users/self/items', { itemId });
}

export function getCharacters(): Promise<Character[]> {
  return get('/users/self/characters');
}
