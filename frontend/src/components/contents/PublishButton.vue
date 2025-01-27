<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  loading?: boolean | string;
  published?: boolean | string;
}>();

const isLoading = computed<boolean>(() => parseBoolean(props.loading) ?? false);
const isPublished = computed<boolean>(() => parseBoolean(props.published) ?? false);

defineEmits<{
  (e: "click"): void;
}>();
</script>

<template>
  <TarButton
    :disabled="isLoading"
    :icon="`fas fa-${isPublished ? 'lock' : 'book'}`"
    :loading="isLoading"
    :text="t(`actions.${isPublished ? 'unpublish' : 'publish'}`)"
    variant="warning"
    @click="$emit('click')"
  />
</template>
