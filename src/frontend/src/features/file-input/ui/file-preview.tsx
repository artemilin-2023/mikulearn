import { Modal } from '@mantine/core';
import { observer } from 'mobx-react-lite';

import { fileInputStore } from '@features/file-input';

type Props = {
  opened: boolean;
  onClose: () => void;
};

export const FilePreview = observer(({ opened, onClose }: Props) => {
  if (!fileInputStore.file) {
    return null;
  }

  const fileUrl: string = URL.createObjectURL(fileInputStore.file);

  return (
    <Modal
      opened={opened}
      onClose={onClose}
      title="Предпросмотр файла"
      size="80%"
      padding="md"
    >
      <iframe
        src={fileUrl}
        style={{ width: '100%', height: '80vh', border: 'none' }}
        title="File Preview"
      />
    </Modal>
  );
});
