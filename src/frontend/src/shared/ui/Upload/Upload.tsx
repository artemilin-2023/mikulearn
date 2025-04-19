import { useState } from 'react';
import { FileButton, Button, Group, Text } from '@mantine/core';

export const UploadForm = () => {
    const [file, setFile] = useState<File | null>(null);
    return (
      <>
        <Group justify="center">
          <FileButton onChange={setFile} accept="image/png,image/jpeg">
            {(props) => <Button {...props}>Upload image</Button>}
          </FileButton>
        </Group>
  
        {file && (
          <Text size="sm" ta="center" mt="sm">
            Picked file: {file.name}
          </Text>
        )}
      </>
    );
};
