<script setup lang="ts">
import { useToggle } from '@vueuse/core'

import { useAsyncCallback } from '~/composables/utils/use-async-callback'
import {
  activateCharacter,
  deleteCharacter,
  updateCharacter,
} from '~/services/characters-service'
import { notify } from '~/services/notification-service'
import { t } from '~/services/translate-service'
import { useUserStore } from '~/stores/user'

definePage({
  meta: {
    layout: 'default',
    roles: ['User', 'Moderator', 'Admin'],
  },
})

const userStore = useUserStore()
const { characters, user } = toRefs(userStore)

const route = useRoute('CharactersId')
const router = useRouter()

const currentCharacterId = computed(() =>
  route.params.id !== undefined ? Number(route.params.id) : undefined,
)

const currentCharacter = computed(() =>
  characters.value.find(char => char.id === currentCharacterId.value),
)
const currentCharacterIsActive = computed(
  () => user.value?.activeCharacterId === currentCharacterId.value,
)

const { execute: onUpdateCharacter } = useAsyncCallback(async ({ name }: { name: string }) => {
  if (currentCharacter.value === undefined) { return }

  if (name !== currentCharacter.value.name) {
    userStore.replaceCharacter(await updateCharacter(currentCharacter.value!.id, { name }))
  }

  notify(t('character.settings.update.notify.success'))
})

const { execute: onDeleteCharacter } = useAsyncCallback(async () => {
  if (currentCharacter.value === undefined) { return }

  if (currentCharacter.value.id === userStore.user!.activeCharacterId) {
    await activateCharacter(currentCharacter.value.id, false)
    await userStore.fetchUser()
  }

  await deleteCharacter(currentCharacter.value.id)
  notify(t('character.settings.delete.notify.success'))

  const characters = userStore.characters.filter(char => char.id !== currentCharacter.value!.id)

  if (characters.length === 0) {
    await router.replace({ name: 'Root' })
  }
  else {
    await router.replace({
      name: 'CharactersId',
      params: { id: userStore.user!.activeCharacterId || characters[0].id },
    })
  }

  userStore.fetchCharacters()
})

const { execute: onActivateCharacter } = useAsyncCallback(async (id: number, status: boolean) => {
  await activateCharacter(id, status)
  await userStore.fetchUser()

  notify(t('character.settings.update.notify.success'))
})

const [shownCreateCharacterGuideModal, toggleCreateCharacterGuideModal] = useToggle()

const { execute: onCreateNewCharacter } = useAsyncCallback(async () => {
  if (user.value?.activeCharacterId !== null && user.value?.activeCharacterId !== undefined) {
    await activateCharacter(user.value.activeCharacterId, false)
    await userStore.fetchUser()
  }

  toggleCreateCharacterGuideModal(true)
})

if (userStore.characters.length === 0) {
  await userStore.fetchCharacters()
}
</script>

<template>
  <div class="container relative py-6">
    <div
      v-if="currentCharacter !== undefined"
      id="character-top-navbar"
      class="mb-16 grid grid-cols-3 items-center justify-between gap-4"
    >
      <div class="order-1 flex items-center gap-4">
        <VDropdown
          :triggers="['click']"
          placement="bottom-end"
        >
          <template #default="{ shown }">
            <OButton
              variant="primary"
              outlined
              size="lg"
            >
              <CharacterMedia
                :character="currentCharacter"
                :is-active="currentCharacter.id === user?.activeCharacterId"
              />

              <Divider inline />

              <OIcon
                icon="chevron-down"
                size="lg"
                :rotation="shown ? 180 : 0"
                class="text-content-400"
              />
            </OButton>
          </template>

          <template #popper="{ hide }">
            <div class="min-w-96">
              <DropdownItem
                v-for="char in characters"
                :key="char.id"
                :checked="char.id === currentCharacterId"
                tag="RouterLink"
                :to="{ name: route.name, params: { id: char.id } }"
                class="justify-between"
                @click="hide"
              >
                <CharacterSelectItem
                  :character="char"
                  :model-value="user!.activeCharacterId === char.id"
                  @update:model-value="(val: boolean) => onActivateCharacter(char.id, val)"
                />
              </DropdownItem>

              <DropdownItem
                class="text-primary hover:text-primary-hover"
                @click="
                  () => {
                    onCreateNewCharacter();
                    hide();
                  }
                "
              >
                <OIcon
                  icon="add"
                  size="lg"
                />
                {{ $t('character.create.title') }}
              </DropdownItem>
            </div>
          </template>
        </VDropdown>

        <Modal>
          <OButton
            v-tooltip="$t('character.settings.update.title')"
            size="xl"
            icon-right="edit"
            rounded
            variant="secondary"
            outlined
          />
          <template #popper="{ hide }">
            <div class="min-w-[480px] max-w-2xl space-y-14 px-12 py-11">
              <CharacterEditForm
                :character="currentCharacter"
                :active="currentCharacterIsActive"
                @cancel="hide"
                @confirm="
                  data => {
                    onUpdateCharacter(data);
                    hide();
                  }
                "
              />

              <i18n-t
                scope="global"
                keypath="character.settings.delete.title"
                tag="div"
                class="text-center"
              >
                <template #link>
                  <Modal>
                    <span class="cursor-pointer text-status-danger hover:text-opacity-80">
                      {{ $t('character.settings.delete.link') }}
                    </span>
                    <template #popper="{ hide }">
                      <ConfirmActionForm
                        :title="$t('character.settings.delete.dialog.title')"
                        :description="$t('character.settings.delete.dialog.desc')"
                        :name="currentCharacter.name"
                        :confirm-label="$t('action.delete')"
                        @cancel="hide"
                        @confirm="
                          () => {
                            onDeleteCharacter();
                            hide();
                          }
                        "
                      />
                    </template>
                  </Modal>
                </template>
              </i18n-t>
            </div>
          </template>
        </Modal>
      </div>
    </div>

    <RouterView v-slot="{ Component }">
      <Suspense>
        <component :is="Component" />
        <template #fallback>
          <OLoading
            full-page
            active
            icon-size="xl"
          />
        </template>
      </Suspense>
    </RouterView>

    <CharacterCreateModal
      :shown="shownCreateCharacterGuideModal"
      @apply-hide="toggleCreateCharacterGuideModal(false)"
    />
  </div>
</template>
