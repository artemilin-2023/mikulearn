import { 
  Card, 
  Grid, 
  Text, 
  Title, 
  Tabs, 
  Group, 
  RingProgress, 
  Progress, 
  Button, 
  Container, 
  Paper, 
  Box,
  Flex
} from '@mantine/core';
import { useEffect, useRef } from 'react';
import styles from './personal-cabinet.module.css';

interface Test {
  id: number;
  title: string;
  score: number;
  totalQuestions: number;
  completed: boolean;
}

interface Note {
  id: number;
  title: string;
  lastAccessed: string;
  progress: number;
}

export const PersonalCabinetPage = () => {
  const gridRef = useRef<HTMLDivElement>(null);

  useEffect(() => {
    const timeout = setTimeout(() => {
      if (gridRef.current) {
        gridRef.current.classList.add('visible');
      }
    }, 100);

    return () => clearTimeout(timeout);
  }, []);

  const tests: Test[] = [
    { id: 1, title: 'Основы JavaScript', score: 85, totalQuestions: 20, completed: true },
    { id: 2, title: 'HTML и CSS', score: 90, totalQuestions: 15, completed: true },
    { id: 3, title: 'Реакт компоненты', score: 0, totalQuestions: 25, completed: false },
    { id: 4, title: 'TypeScript основы', score: 0, totalQuestions: 30, completed: false },
  ];

  const notes: Note[] = [
    { id: 1, title: 'Введение в JavaScript', lastAccessed: '2023-05-10', progress: 100 },
    { id: 2, title: 'Основы React', lastAccessed: '2023-05-15', progress: 75 },
    { id: 3, title: 'CSS продвинутый уровень', lastAccessed: '2023-05-08', progress: 60 },
    { id: 4, title: 'TypeScript для разработчиков', lastAccessed: '2023-05-01', progress: 30 },
  ];
  
  const completedTests = tests.filter(test => test.completed).length;
  const avgScore = Math.round(tests.filter(test => test.completed).reduce((acc, test) => acc + test.score, 0) / 
                  (tests.filter(test => test.completed).length || 1));
  const completedNotes = notes.filter(note => note.progress === 100).length;
  const totalProgress = Math.round(notes.reduce((acc, note) => acc + note.progress, 0) / notes.length);
  
  return (
    <Container size="lg" className={styles.personalCabinet}>
      <Title order={1} mb="md">
        <span>Личный кабинет</span>
      </Title>
      
      <Flex gap="md">
        {/* Main Content - Now on the Left */}
        <Paper shadow="sm" p="lg" radius="md" className={styles.mainContent} style={{ flex: 1 }}>
          <Tabs defaultValue="tests" styles={{ tab: { fontWeight: 500 } }}>
            <Tabs.List>
              <Tabs.Tab p="lg" value="tests">Тесты</Tabs.Tab>
              <Tabs.Tab p="lg" value="notes">Конспекты</Tabs.Tab>
            </Tabs.List>

            <Tabs.Panel value="tests" pt="md">
              {tests.map((test, index) => (
                <Card 
                  key={test.id} 
                  shadow="sm" 
                  p="lg" 
                  radius="md" 
                  withBorder 
                  mb="md" 
                  className={styles.testCard}
                  style={{ animationDelay: `${index * 0.1}s` }}
                >
                  <Grid>
                    <Grid.Col span={{ base: 12, md: 8 }}>
                      <Box className={styles.testInfo}>
                        <Title order={4}>{test.title}</Title>
                        <Text c="dimmed">Вопросов: {test.totalQuestions}</Text>
                      </Box>
                    </Grid.Col>
                    <Grid.Col span={{ base: 12, md: 4 }} className={styles.testStatus}>
                      {test.completed ? (
                        <Box>
                          <RingProgress
                            size={80}
                            thickness={8}
                            sections={[{ 
                              value: test.score, 
                              color: test.score > 80 ? 'green' : test.score > 60 ? 'cyan' : 'orange' 
                            }]}
                            label={
                              <Text ta="center" size="lg" fw={700}>{test.score}</Text>
                            }
                          />
                          <Text ta="center" size="sm" fw={500} className={styles.scoreText}>Ваш балл</Text>
                        </Box>
                      ) : (
                        <Button fullWidth className={styles.startTestBtn}>
                          Начать тест
                        </Button>
                      )}
                    </Grid.Col>
                  </Grid>
                </Card>
              ))}
            </Tabs.Panel>
            
            <Tabs.Panel value="notes" pt="md">
              {notes.map((note, index) => (
                <Card 
                  key={note.id} 
                  shadow="sm" 
                  p="lg" 
                  radius="md" 
                  withBorder 
                  mb="md" 
                  className={styles.noteCard}
                  style={{ animationDelay: `${index * 0.1}s` }}
                >
                  <Grid>
                    <Grid.Col span={{ base: 12, md: 8 }}>
                      <Box className={styles.noteInfo}>
                        <Title order={4}>{note.title}</Title>
                        <Group>
                          <div className={styles.icon}>🕒</div>
                          <Text c="dimmed">Последний доступ: {note.lastAccessed}</Text>
                        </Group>
                      </Box>
                    </Grid.Col>
                    <Grid.Col span={{ base: 12, md: 4 }}>
                      <Progress 
                        value={note.progress} 
                        size="md" 
                        mb="sm" 
                        color={note.progress === 100 ? 'green' : 'cyan'} 
                        radius="md"
                        striped={note.progress < 100}
                        animated={note.progress < 100}
                      />
                      <Button 
                        fullWidth 
                        variant="gradient" 
                        gradient={{ from: 'teal', to: 'lime', deg: 105 }} 
                        className={styles.continueBtn}
                      >
                        {note.progress === 100 ? 'Повторить' : 'Продолжить'}
                      </Button>
                    </Grid.Col>
                  </Grid>
                </Card>
              ))}
            </Tabs.Panel>
          </Tabs>
        </Paper>
        
        {/* Dashboard Section - Now on the Right with fixed width */}
        <Paper p="md" radius="md" className={styles.dashboardSection} style={{ width: '300px', flexShrink: 0 }}>
          <Title order={3} mb="md">Дашборд</Title>
          <div ref={gridRef} className="grid-animation-container">
            <Card shadow="sm" p="lg" radius="md" withBorder className={styles.statCard} mb="md">
              <Group>
                <Box>
                  <Text size="sm" c="dimmed">Завершенные тесты</Text>
                  <Group>
                    <div className={styles.icon}>📊</div>
                    <Text size="xl" fw={700}>{completedTests} / {tests.length}</Text>
                  </Group>
                </Box>
              </Group>
            </Card>
            
            <Card shadow="sm" p="lg" radius="md" withBorder className={styles.statCard} mb="md">
              <Group>
                <Box>
                  <Text size="sm" c="dimmed">Средний балл</Text>
                  <Text size="xl" fw={700}>{avgScore}%</Text>
                </Box>
                <RingProgress 
                  size={80}
                  thickness={8}
                  sections={[{ value: avgScore, color: 'cyan' }]}
                  label={
                    <Text ta="center" size="lg" fw={700}>{avgScore}</Text>
                  }
                />
              </Group>
            </Card>
            
            <Card shadow="sm" p="lg" radius="md" withBorder className={styles.statCard} mb="md">
              <Group>
                <Box>
                  <Text size="sm" c="dimmed">Конспектов изучено</Text>
                  <Group>
                    <div className={styles.icon}>📚</div>
                    <Text size="xl" fw={700}>{completedNotes} / {notes.length}</Text>
                  </Group>
                </Box>
              </Group>
            </Card>
            
            <Card shadow="sm" p="lg" radius="md" withBorder className={styles.statCard}>
              <Group>
                <Box>
                  <Text size="sm" c="dimmed">Общий прогресс</Text>
                  <Text size="xl" fw={700}>{totalProgress}%</Text>
                </Box>
                <RingProgress 
                  size={80}
                  thickness={8}
                  sections={[{ value: totalProgress, color: 'green' }]}
                  label={
                    <Text ta="center" size="lg" fw={700}>{totalProgress}</Text>
                  }
                />
              </Group>
            </Card>
          </div>
        </Paper>
      </Flex>
    </Container>
  );
};
