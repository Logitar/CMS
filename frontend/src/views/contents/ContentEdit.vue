<script setup lang="ts">
import { TarButton, TarTab, TarTabs } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import ContentLocaleEdit from "@/components/contents/ContentLocaleEdit.vue";
import LanguageSelect from "@/components/languages/LanguageSelect.vue";
import PublishButton from "@/components/contents/PublishButton.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import UnpublishButton from "@/components/contents/UnpublishButton.vue";
import type { Actor } from "@/types/actor";
import type { ApiError } from "@/types/api";
import type { Content, ContentLocale } from "@/types/contents";
import type { Language } from "@/types/languages";
import { StatusCodes } from "@/enums/statusCodes";
import { handleErrorKey } from "@/inject/App";
import { publishAllContent, readContent, unpublishAllContent } from "@/api/contents";
import { useAccountStore } from "@/stores/account";
import { useToastStore } from "@/stores/toast";

const account = useAccountStore();
const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const content = ref<Content>();
const isPublishing = ref<boolean>(false);
const language = ref<Language>();

const canPublish = computed<boolean>(() => content.value?.locales.some((locale) => locale.revision !== (locale.publishedRevision ?? 0)) ?? false);
const canUnpublish = computed<boolean>(() => content.value?.locales.some((locale) => locale.isPublished) ?? false);
const isInvariant = computed<boolean>(() => content.value?.contentType.isInvariant ?? false);
const languageIds = computed<string[]>(() => (content.value?.locales ?? []).map((locale) => locale.language?.id ?? ""));
const locales = computed<ContentLocale[]>(() => (content.value ? content.value.locales.filter((locale) => Boolean(locale.language)) : []).sort(compare));

function addLocale(): void {
  if (content.value && language.value) {
    const actor: Actor = account.getActor();
    const now: string = new Date().toISOString();
    const locale: ContentLocale = {
      content: content.value,
      language: language.value,
      uniqueName: "",
      displayName: undefined,
      description: undefined,
      fieldValues: [],
      createdBy: actor,
      createdOn: now,
      updatedBy: actor,
      updatedOn: now,
      isPublished: false,
      revision: 0,
      publishedBy: undefined,
      publishedOn: undefined,
    };
    content.value.locales.push(locale);
    language.value = undefined;
  }
}

function compare(left: ContentLocale, right: ContentLocale): -1 | 0 | 1 {
  if (left.language && right.language) {
    if (left.language.locale.displayName < right.language.locale.displayName) {
      return -1;
    } else if (left.language.locale.displayName > right.language.locale.displayName) {
      return 1;
    }
  }
  return 0;
}

async function onPublish(): Promise<void> {
  if (content.value && !isPublishing.value) {
    isPublishing.value = true;
    try {
      const publishedContent: Content = await publishAllContent(content.value.id);
      onPublished(publishedContent);
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isPublishing.value = false;
    }
  }
}
function onPublished(content: Content): void {
  setModel(content);
  toasts.success("contents.items.published.success");
}

async function onUnpublish(): Promise<void> {
  if (content.value && !isPublishing.value) {
    isPublishing.value = true;
    try {
      const unpublishedContent: Content = await unpublishAllContent(content.value.id);
      onUnpublished(unpublishedContent);
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isPublishing.value = false;
    }
  }
}
function onUnpublished(content: Content): void {
  setModel(content);
  toasts.success("contents.items.unpublished");
}

function onSaved(model: Content, language?: Language): void {
  if (content.value) {
    content.value.version = model.version;
    content.value.updatedBy = { ...model.updatedBy };
    content.value.updatedOn = model.updatedOn;

    if (language) {
      const locale: ContentLocale | undefined = model.locales.find((locale) => locale.language?.id === language.id);
      if (locale) {
        const index: number = content.value.locales.findIndex((locale) => locale.language?.id === language.id);
        if (index < 0) {
          content.value.locales.push({ ...locale });
        } else {
          content.value.locales.splice(index, 1, { ...locale });
        }
      }
    } else {
      content.value.invariant = { ...model.invariant };
    }
  } else {
    setModel(model);
  }
  toasts.success("contents.items.updated");
}

function setModel(model: Content): void {
  content.value = model;
}

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const content: Content = await readContent(id);
      setModel(content);
    }
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === StatusCodes.NotFound) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <main class="container">
    <template v-if="content">
      <h1>{{ content.invariant.displayName ?? content.invariant.uniqueName }}</h1>
      <StatusDetail v-if="!content.contentType.isInvariant" :aggregate="content" />
      <ContentLocaleEdit
        v-if="isInvariant"
        :content="content"
        :locale="content.invariant"
        @error="handleError"
        @published="onPublished"
        @saved="onSaved"
        @unpublished="onUnpublished"
      />
      <template v-else>
        <div class="mb-3">
          <PublishButton class="me-1" :disabled="!canPublish" :loading="isPublishing" @click="onPublish" />
          <UnpublishButton class="ms-1" :disabled="!canUnpublish" :loading="isPublishing" @click="onUnpublish" />
        </div>
        <LanguageSelect :exclude="languageIds" no-status :model-value="language?.id" @selected="language = $event">
          <template #append>
            <TarButton :disabled="!language" icon="fas fa-plus" :text="t('actions.add')" variant="success" @click="addLocale" />
          </template>
        </LanguageSelect>
        <TarTabs>
          <TarTab active id="invariant" :title="t('contents.items.invariant')">
            <ContentLocaleEdit
              :content="content"
              :locale="content.invariant"
              @error="handleError"
              @published="onPublished"
              @saved="onSaved"
              @unpublished="onUnpublished"
            />
          </TarTab>
          <TarTab v-for="locale in locales" :key="locale.language?.id" :id="locale.language?.id" :title="locale.language?.locale.displayName">
            <ContentLocaleEdit
              :content="content"
              :locale="locale"
              @error="handleError"
              @published="onPublished"
              @saved="onSaved($event, locale.language)"
              @unpublished="onUnpublished"
            />
          </TarTab>
        </TarTabs>
      </template>
    </template>
  </main>
</template>
