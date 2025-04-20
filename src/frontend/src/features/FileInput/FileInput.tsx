import { useState } from 'react';
import { Dropzone, FileRejection } from '@mantine/dropzone';
import { Group, Text, rem, Progress, Paper, ActionIcon } from '@mantine/core';
import { FiUpload, FiX, FiFile, FiTrash } from 'react-icons/fi';
import { useStore } from '@shared/store/store';
import { observer } from 'mobx-react-lite';
import styles from './FileInput.module.css';

export const FileInput = observer(() => {
    const store = useStore();
    const [loading, setLoading] = useState(false);
    const [progress, setProgress] = useState(0);

    const handleDrop = (files: File[]) => {
        if (files.length > 0) {
            const file = files[0];
            setLoading(true);
            
            const interval = setInterval(() => {
                setProgress((prev) => {
                    if (prev >= 100) {
                        clearInterval(interval);
                        setLoading(false);
                        store.setFile(file);
                        return 100;
                    }
                    return prev + 10;
                });
            }, 100);
        }
    };

    const handleReject = (fileRejections: FileRejection[]) => {
        console.error('File rejected', fileRejections);
    };

    const clearFile = () => {
        // @ts-expect-error - need to handle null case in the store
        store.setFile(null);
        setProgress(0);
        setLoading(false);
    };

    return (
        <Paper shadow="xs">
            {store.file ? (
                <div className={styles.fileInfo}>
                    <Group gap="sm">
                        <FiFile size="32px" />
                        <div className={styles.fileDetails}>
                            <Text fw={500} size="sm">{store.file.name}</Text>
                            <Text size="xs" c="dimmed">{(store.file.size / 1024 / 1024).toFixed(2)} MB</Text>
                        </div>
                        <ActionIcon 
                            color="red" 
                            variant="subtle" 
                            onClick={clearFile}
                            className={styles.clearButton}
                        >
                            <FiTrash size="1rem" />
                        </ActionIcon>
                    </Group>
                </div>
            ) : (
                <Dropzone
                    onDrop={handleDrop}
                    onReject={handleReject}
                    maxSize={10 * 1024 * 1024}
                    accept={{'application/pdf': [], 'application/msword': [], 'application/vnd.openxmlformats-officedocument.wordprocessingml.document': []}}
                    loading={loading}
                    className={styles.dropzone}
                >
                    <Group justify="center" gap="xl" style={{ minHeight: rem(140), pointerEvents: 'none' }}>
                        <Dropzone.Accept>
                            <FiUpload
                                size="50px"
                                color="var(--mantine-color-blue-6)"
                            />
                        </Dropzone.Accept>
                        <Dropzone.Reject>
                            <FiX
                                size="50px"
                                color="var(--mantine-color-red-6)"
                            />
                        </Dropzone.Reject>
                        <Dropzone.Idle>
                            <FiUpload
                                size="50px"
                            />
                        </Dropzone.Idle>

                        <div>
                            <Text size="xl" inline>
                                Перетащите файл сюда или нажмите для выбора
                            </Text>
                            <Text size="sm" c="dimmed" inline mt={7}>
                                Файл не должен превышать 10MB
                            </Text>
                        </div>
                    </Group>
                </Dropzone>
            )}
            
            {loading && (
                <Progress 
                    value={progress} 
                    size="sm" 
                    color="blue" 
                    striped 
                    animated 
                    mt="md"
                />
            )}

        </Paper>
    );
});